using System.Runtime.InteropServices;
using System.Security;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Exposures;
using cCoder.ContentManagement.Rendering.Models;
using cCoder.ContentManagement.Services;
using cCoder.ContentManagement.Services.Orchestrations;
using App = cCoder.Data.Models.CMS.App;
using Content = cCoder.Data.Models.CMS.Content;
using Page = cCoder.Data.Models.CMS.Page;
using PageInfo = cCoder.Data.Models.CMS.PageInfo;
using User = cCoder.Data.Models.Security.User;
using RenderResult = cCoder.ContentManagement.Models.RenderResult;

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
    private User User => authorizationBroker.GetCurrentUser();

    public PageRenderResponse Render(PageRenderRequest request)
    {
        ValidateRequest(request, "request");

        try
        {
            ResolvedPageRenderDefaults defaults = ResolveDefaults(request);
            RenderResult page = Render(defaults.App.Id, request.Path ?? string.Empty, defaults.Theme, defaults.Culture, request.Edit);

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
            return RenderError(request);
        }
    }

    public PageRenderResponse RenderError(PageRenderRequest request)
    {
        ValidateRequest(request, "request");
        ValidateException(request.Exception, "Exception");

        ResolvedPageRenderDefaults defaults = ResolveDefaults(request);
        RenderResult page = Render(defaults.App.Id, "Error", defaults.Theme, defaults.Culture);

        page.BodyHtml = page.BodyHtml.Replace("[problem[message]]", request.Exception.Message);
        page.BodyHtml = page.BodyHtml.Replace("[problem[detail]]", request.Exception.StackTrace ?? string.Empty);
        page.BodyHtml = page.BodyHtml.Replace("[problem[url]]", request.RequestUrl ?? string.Empty);

        return new PageRenderResponse
        {
            App = defaults.App,
            Page = page,
            Theme = defaults.Theme,
            Culture = defaults.Culture,
            Edit = false
        };
    }

    public RenderResult Render(int appId, string path, string theme, string culture, bool edit = false)
    {
        ValidateAppId(appId, "appId");
        ValidateTheme(theme, "theme");

        path ??= string.Empty;
        culture ??= User.DefaultCultureId;

        App app = ResolveAppById(appId);

        if (app == null)
        {
            throw new SecurityException("Unknown Domain!");
        }

        string normalizedPath = path.ToLowerInvariant();
        Page page = pageOrchestrationService.GetAll(ignoreFilters: true)
            .Where(existingPage => existingPage.AppId == appId && existingPage.Path.ToLower() == normalizedPath)
            .FirstOrDefault();

        if (page != null)
        {
            page.App = app;
            HydratePageForRender(page);
        }

        if (page == null)
        {
            RenderResult renderResult = pageRenderOrchestrationService.Render(
                CreateMissingPage(app, path, culture),
                User,
                theme,
                culture);

            renderResult.StatusCode = 404;
            return renderResult;
        }

        if (!ContentManagementModelLogic.UserCan(page, User, "page_read") && !authorizationBroker.IsAdminOfApp(appId))
        {
            Page gatedPage = CreateGatedPage(page);
            gatedPage.App = app;

            return pageRenderOrchestrationService.Render(gatedPage, User, theme, culture);
        }

        return pageRenderOrchestrationService.Render(
            page,
            User,
            theme,
            culture,
            edit && ContentManagementModelLogic.UserCan(page, User, "page_update"));
    }

    private ResolvedPageRenderDefaults ResolveDefaults(PageRenderRequest request)
    {
        ValidateHost(request.Host);

        App app = ResolveAppByDomain(request.Host)
            ?? throw new InvalidOperationException("Domain Not found!");

        if (app.Id < 1)
        {
            throw new InvalidOperationException("Domain Not found!");
        }

        return new ResolvedPageRenderDefaults
        {
            App = app,
            Theme = string.IsNullOrWhiteSpace(request.Theme)
                ? app.DefaultTheme ?? "Default"
                : request.Theme,
            Culture = string.IsNullOrWhiteSpace(request.Culture)
                ? app.DefaultCultureId ?? string.Empty
                : request.Culture
        };
    }

    private App ResolveAppByDomain(string domain)
    {
        App app = appOrchestrationService.GetAll(ignoreFilters: false)
            .Where(existingApp => existingApp.Domain == domain)
            .Select(existingApp => new App
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
            PopulateRenderCollections(app);
        }

        return app;
    }

    private App ResolveAppById(int appId)
    {
        App app = appOrchestrationService.GetAll(ignoreFilters: false)
            .Where(existingApp => existingApp.Id == appId)
            .Select(existingApp => new App
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
            PopulateRenderCollections(app);
        }

        return app;
    }

    private void PopulateRenderCollections(App app)
    {
        app.Layouts = layoutOrchestrationService.GetAll(ignoreFilters: false)
            .Where(layout => layout.AppId == app.Id)
            .ToArray();

        app.Templates = templateOrchestrationService.GetAll(ignoreFilters: false)
            .Where(template => template.AppId == app.Id)
            .ToArray();

        app.Resources = resourceOrchestrationService.GetAll(ignoreFilters: false)
            .Where(resource => resource.AppId == app.Id)
            .ToArray();

        app.Components = componentOrchestrationService.GetAll(ignoreFilters: false)
            .Where(component => component.AppId == app.Id)
            .ToArray();

        app.Scripts = scriptOrchestrationService.GetAll(ignoreFilters: false)
            .Where(script => script.AppId == app.Id)
            .ToArray();

        app.Pages = pageOrchestrationService.GetAll(ignoreFilters: false)
            .Where(page => page.AppId == app.Id)
            .Select(page => new Page
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
        page.PageInfo ??= pageInfoOrchestrationService.GetAll(ignoreFilters: true)
            .Where(pageInfo => pageInfo.PageId == page.Id)
            .ToArray();

        page.Contents ??= contentOrchestrationService.GetAll(ignoreFilters: true)
            .Where(content => content.PageId == page.Id)
            .ToArray();

        page.Roles ??= pageRoleOrchestrationService.GetAll(ignoreFilters: true)
            .Where(pageRole => pageRole.PageId == page.Id)
            .ToArray();
    }

    private static Page CreateMissingPage(App app, string path, string culture) =>
        new()
        {
            App = app,
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

    private static Page CreateGatedPage(Page page)
    {
        string[] contentNames = page.Contents?
            .Select(content => content.Name)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray()
            ?? [];

        if (contentNames.Length == 0)
        {
            contentNames = ["body"];
        }

        List<Content> gatedContents = new(contentNames.Length);
        CollectionsMarshal.SetCount(gatedContents, contentNames.Length);

        for (int index = 0; index < contentNames.Length; index++)
        {
            CollectionsMarshal.AsSpan(gatedContents)[index] = new Content
            {
                CultureId = string.Empty,
                Html = "[component[login]]",
                Name = contentNames[index]
            };
        }

        return new Page
        {
            Id = page.Id,
            AppId = page.AppId,
            ParentId = page.ParentId,
            Path = page.Path,
            Order = page.Order,
            ShowOnMenus = page.ShowOnMenus,
            Name = page.Name,
            ResourceKey = page.ResourceKey,
            Layout = page.Layout,
            App = page.App,
            PageInfo = page.PageInfo,
            Contents = gatedContents,
            Roles = page.Roles,
            Pages = page.Pages
        };
    }
}
