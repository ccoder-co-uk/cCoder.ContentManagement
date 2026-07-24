// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Runtime.InteropServices;
using System.Security;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Exposures;
using cCoder.ContentManagement.Rendering.Models;
using cCoder.ContentManagement.Services;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Coordinations;

internal sealed partial class PageRenderCoordinationService(
    IAuthorizationBroker authorizationBroker,
    IAppOrchestrationService appOrchestrationService,
    ILayoutOrchestrationService layoutOrchestrationService,
    ITemplateOrchestrationService templateOrchestrationService,
    IResourceOrchestrationService resourceOrchestrationService,
    IComponentOrchestrationService componentOrchestrationService,
    IScriptOrchestrationService scriptOrchestrationService,
    IPageOrchestrationService pageOrchestrationService,
    IContentOrchestrationService contentOrchestrationService,
    IPageInfoOrchestrationService pageInfoOrchestrationService,
    IPageRoleOrchestrationService pageRoleOrchestrationService,
    IPageRenderOrchestrationService pageRenderOrchestrationService) : IPageRenderCoordinationService
{
    private User User =>
        authorizationBroker.GetCurrentUser();

    public PageRenderResponse Render(PageRenderRequest request)
    {
        ValidateRequest(request: request, parameterName: "request");

        try
        {
            ResolvedPageRenderDefaults defaults = ResolveDefaults(request: request);
            RenderResult page = RenderRenderResult(appId: defaults.App.Id, path: request.Path ?? string.Empty, theme: defaults.Theme, culture: defaults.Culture, edit: request.Edit);

            return new PageRenderResponse
            {
                App = defaults.App,
                Page = page,
                Theme = defaults.Theme,
                Culture = defaults.Culture,
                Edit = request.Edit
            };
        }
        catch (Exception exception)
        {
            request.Exception = exception;
            return RenderError(request: request);
        }
    }

    public PageRenderResponse RenderError(PageRenderRequest request)
    {
        ValidateRequest(request: request, parameterName: "request");
        ValidateException(exception: request.Exception, parameterName: "Exception");

        ResolvedPageRenderDefaults defaults = ResolveDefaults(request: request);
        RenderResult page = RenderRenderResult(appId: defaults.App.Id, path: "Error", theme: defaults.Theme, culture: defaults.Culture);

        page.BodyHtml = page.BodyHtml.Replace(oldValue: "[problem[message]]", newValue: request.Exception.Message);
        page.BodyHtml = page.BodyHtml.Replace(oldValue: "[problem[detail]]", newValue: request.Exception.StackTrace ?? string.Empty);
        page.BodyHtml = page.BodyHtml.Replace(oldValue: "[problem[url]]", newValue: request.RequestUrl ?? string.Empty);

        return new PageRenderResponse
        {
            App = defaults.App,
            Page = page,
            Theme = defaults.Theme,
            Culture = defaults.Culture,
            Edit = false
        };
    }

    public RenderResult RenderRenderResult(int appId, string path, string theme, string culture, bool edit = false)
    {
        ValidateAppId(appId: appId, parameterName: "appId");
        ValidateTheme(theme: theme, parameterName: "theme");

        path ??= string.Empty;
        culture ??= User.DefaultCultureId;

        App app = ResolveAppById(appId: appId);

        if (app == null)
        {
            throw new SecurityException(message: "Unknown Domain!");
        }

        string normalizedPath = path.ToLowerInvariant();

        Page page = pageOrchestrationService.GetAllPage(ignoreFilters: true)
            .Where(predicate: existingPage => existingPage.AppId == appId && existingPage.Path.ToLower() == normalizedPath)
            .FirstOrDefault();

        if (page != null)
        {
            page.App = app;
            HydratePageForRender(page: page);
        }

        if (page == null)
        {
            RenderResult renderResult = pageRenderOrchestrationService.RenderPageUserRenderResult(
page: CreateMissingPage(newApp: app, path: path, culture: culture),
user: User,
theme: theme,
culture: culture);

            renderResult.StatusCode = 404;
            return renderResult;
        }

        if (!ContentManagementModelLogic.UserCan(page: page, user: User, privilege: "page_read") && !authorizationBroker.IsAdminOfApp(appId: appId))
        {
            Page gatedPage = CreateGatedPage(newPage: page);
            gatedPage.App = app;

            return pageRenderOrchestrationService.RenderPageUserRenderResult(page: gatedPage, user: User, theme: theme, culture: culture);
        }

        return pageRenderOrchestrationService.RenderPageUserRenderResult(
page: page,
user: User,
theme: theme,
culture: culture,
edit: edit && ContentManagementModelLogic.UserCan(page: page, user: User, privilege: "page_update"));
    }

    private ResolvedPageRenderDefaults ResolveDefaults(PageRenderRequest request)
    {
        ValidateHost(host: request.Host);

        App app = ResolveAppByDomain(domain: request.Host)
            ?? throw new InvalidOperationException(message: "Domain Not found!");

        if (app.Id < 1)
        {
            throw new InvalidOperationException(message: "Domain Not found!");
        }

        return new ResolvedPageRenderDefaults
        {
            App = app,
            Theme = string.IsNullOrWhiteSpace(value: request.Theme)
                ? app.DefaultTheme ?? "Default"
                : request.Theme,
            Culture = string.IsNullOrWhiteSpace(value: request.Culture)
                ? app.DefaultCultureId ?? string.Empty
                : request.Culture
        };
    }

    private App ResolveAppByDomain(string domain)
    {
        App app = appOrchestrationService.GetAllApp(ignoreFilters: false)
            .Where(predicate: existingApp => existingApp.Domain == domain)
            .Select(selector: existingApp => new App
            {
                Id = existingApp.Id,
                DefaultCultureId = existingApp.DefaultCultureId,
                TenantId = existingApp.TenantId,
                Name = existingApp.Name,
                Domain = existingApp.Domain,
                DefaultTheme = existingApp.DefaultTheme,
                ConfigJson = existingApp.ConfigJson
            })
            .FirstOrDefault();

        if (app != null)
        {
            PopulateRenderCollections(app: app);
        }

        return app;
    }

    private App ResolveAppById(int appId)
    {
        App app = appOrchestrationService.GetAllApp(ignoreFilters: false)
            .Where(predicate: existingApp => existingApp.Id == appId)
            .Select(selector: existingApp => new App
            {
                Id = existingApp.Id,
                DefaultCultureId = existingApp.DefaultCultureId,
                TenantId = existingApp.TenantId,
                Name = existingApp.Name,
                Domain = existingApp.Domain,
                DefaultTheme = existingApp.DefaultTheme,
                ConfigJson = existingApp.ConfigJson
            })
            .FirstOrDefault();

        if (app != null)
        {
            PopulateRenderCollections(app: app);
        }

        return app;
    }

    private void PopulateRenderCollections(App app)
    {
        app.Layouts = layoutOrchestrationService.GetAllLayout(ignoreFilters: false)
            .Where(predicate: layout => layout.AppId == app.Id)
            .ToArray();

        app.Templates = templateOrchestrationService.GetAllTemplate(ignoreFilters: false)
            .Where(predicate: template => template.AppId == app.Id)
            .ToArray();

        app.Resources = resourceOrchestrationService.GetAllResource(ignoreFilters: false)
            .Where(predicate: resource => resource.AppId == app.Id)
            .ToArray();

        app.Components = componentOrchestrationService.GetAllComponent(ignoreFilters: false)
            .Where(predicate: component => component.AppId == app.Id)
            .ToArray();

        app.Scripts = scriptOrchestrationService.GetAllScript(ignoreFilters: false)
            .Where(predicate: script => script.AppId == app.Id)
            .ToArray();

        app.Pages = pageOrchestrationService.GetAllPage(ignoreFilters: false)
            .Where(predicate: page => page.AppId == app.Id)
            .Select(selector: page => new Page
            {
                Id = page.Id,
                ParentId = page.ParentId,
                AppId = page.AppId,
                Order = page.Order,
                ShowOnMenus = page.ShowOnMenus,
                Name = page.Name,
                Path = page.Path,
                ResourceKey = page.ResourceKey,
                Layout = page.Layout,
                PageInfo = page.PageInfo
            })
            .ToArray();
    }

    private void HydratePageForRender(Page page)
    {
        page.PageInfo ??= pageInfoOrchestrationService.GetAllPageInfo(ignoreFilters: true)
            .Where(predicate: pageInfo => pageInfo.PageId == page.Id)
            .ToArray();

        page.Contents ??= contentOrchestrationService.GetAllContent(ignoreFilters: true)
            .Where(predicate: content => content.PageId == page.Id)
            .ToArray();

        page.Roles ??= pageRoleOrchestrationService.GetAllPageRole(ignoreFilters: true)
            .Where(predicate: pageRole => pageRole.PageId == page.Id)
            .ToArray();
    }

    private static Page CreateMissingPage(App newApp, string path, string culture) =>
        new()
        {
            App = newApp,
            Path = path,
            PageInfo =
            [
                new PageInfo
                {
                    Title = "Page Not Found",
                    Description = "Page Not Found",
                    Keywords = "Page Not Found",
                    CultureId = string.Empty
                }
            ],
            Contents =
            [
                new Content
                {
                    CultureId = culture,
                    Name = "body",
                    Html = "[component[NotFound]]"
                }
            ]
        };

    private static Page CreateGatedPage(Page newPage)
    {
        string[] contentNames = newPage.Contents?
            .Select(selector: content => content.Name)
            .Where(predicate: name => !string.IsNullOrWhiteSpace(value: name))
            .Distinct(comparer: StringComparer.OrdinalIgnoreCase)
            .ToArray()
            ?? [];

        if (contentNames.Length == 0)
        {
            contentNames = ["body"];
        }

        List<Content> gatedContents = new(capacity: contentNames.Length);
        CollectionsMarshal.SetCount(list: gatedContents, count: contentNames.Length);

        for (int index = 0; index < contentNames.Length; index++)
        {
            CollectionsMarshal.AsSpan(list: gatedContents)[index] = new Content
            {
                CultureId = string.Empty,
                Html = "[component[login]]",
                Name = contentNames[index]
            };
        }

        return new Page
        {
            Id = newPage.Id,
            AppId = newPage.AppId,
            ParentId = newPage.ParentId,
            Path = newPage.Path,
            Order = newPage.Order,
            ShowOnMenus = newPage.ShowOnMenus,
            Name = newPage.Name,
            ResourceKey = newPage.ResourceKey,
            Layout = newPage.Layout,
            App = newPage.App,
            PageInfo = newPage.PageInfo,
            Contents = gatedContents,
            Roles = newPage.Roles,
            Pages = newPage.Pages
        };
    }
}