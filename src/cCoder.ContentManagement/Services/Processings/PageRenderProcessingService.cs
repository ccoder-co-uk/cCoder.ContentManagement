// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Extensions;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Models.PageRendering;
using cCoder.ContentManagement.Rendering.Services.Orchestrations;
using cCoder.ContentManagement.Services;
using cCoder.ContentManagement.Services.Foundations.Rendering;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

using cCoder.ContentManagement.Services.Foundations;

namespace cCoder.ContentManagement.Services.Processings;

internal sealed partial class PageRenderProcessingService(
    IPageRenderService pageRenderService,
    ContentManagementConfiguration config) : IPageRenderProcessingService
{
    public PageRenderOperation RenderPageRenderOperation(
        PageRenderOperation operation) =>
        TryCatch<PageRenderOperation>(operation: () =>
    {
        ValidateRenderPageRenderOperation(inputs: [operation]);

        operation.Page = RenderPageUserRenderResult(
            page: operation.SourcePage,
            user: operation.User,
            theme: operation.Theme,
            culture: operation.Culture,
            edit: operation.Edit,
            headerOnly: operation.HeaderOnly,
            cacheTemplate: operation.CacheTemplate);

        return operation;
    });

    internal RenderResult RenderPageUserRenderResult(
        Page page,
        User user,
        string theme,
        string culture,
        bool edit = false,
        bool headerOnly = false,
        bool cacheTemplate = false) =>
        TryCatch<RenderResult>(operation: () =>
    {
        ValidateRenderPageUserRenderResult(inputs: [page, user, theme, culture, edit, headerOnly]);
        ValidatePage(page: page, parameterName: "page");
        ValidateUser(user: user, parameterName: "user");
        ValidateTheme(theme: theme, parameterName: "theme");

        PageRenderSession session =
            BuildSession(
                page: page,
                user: user,
                config: config,
                theme: theme,
                culture: culture,
                edit: edit,
                headerOnly: headerOnly,
                cacheTemplate: cacheTemplate);

        PageRenderSession renderedSession =
            pageRenderService.Execute<
                IPageRenderExecutionOrchestrationService,
                PageRenderSession>(
                    name: "PageRenderExecution",
                    operation: service =>
                        service.RenderPageRenderSession(
                            session: session));

        PageRenderResult pageRenderResult = renderedSession.Result;

        return new RenderResult
        {
            AppId = pageRenderResult.AppId,
            PageId = pageRenderResult.PageId,
            ParentId = pageRenderResult.ParentId,
            Theme = pageRenderResult.Theme,
            Culture = pageRenderResult.Culture,
            Edit = pageRenderResult.Edit,
            Path = pageRenderResult.Path,
            Layout = pageRenderResult.Layout,
            Title = pageRenderResult.Title,
            Description = pageRenderResult.Description,
            Keywords = pageRenderResult.Keywords,
            HeaderHtml = pageRenderResult.HeaderHtml,
            BodyHtml = pageRenderResult.BodyHtml,
            StatusCode = pageRenderResult.StatusCode
        };

    });

    private static PageRenderSession BuildSession(
        Page page,
        User user,
        ContentManagementConfiguration config,
        string theme,
        string culture,
        bool edit,
        bool headerOnly,
        bool cacheTemplate)
    {
        App app = page.App ?? throw new InvalidOperationException(message: "page.App is required.");
        string resolvedTheme = string.IsNullOrWhiteSpace(value: theme) ? app.DefaultTheme ?? "Default" : theme;

        string resolvedCulture = string.IsNullOrWhiteSpace(value: culture)
            ? user.DefaultCultureId ?? app.DefaultCultureId ?? string.Empty
            : culture;

        return new PageRenderSession
        {
            Request = new PageRenderEngineRequest
            {
                AppId = app.Id,
                Path = page.Path ?? string.Empty,
                Theme = resolvedTheme,
                Culture = resolvedCulture,
                Edit = edit,
                HeaderOnly = headerOnly,
                CacheTemplate = cacheTemplate
            },
            Config = config,
            App = MapApp(app: app, culture: resolvedCulture),
            Page = MapPage(page: page, culture: resolvedCulture, includeContent: true),
            User = MapUser(user: user),
            Layout = ResolveLayout(app: app, layoutName: page.Layout),
            Resources = MapResources(resources: app.Resources),
            ResourcesByLookup = BuildResourceLookup(resources: app.Resources),
            ComponentsByName = BuildComponentLookup(components: app.Components),
            ScriptsByName = BuildScriptLookup(scripts: app.Scripts)
        };
    }

    private static PageRenderApp MapApp(App app, string culture) =>
        new PageRenderApp
        {
            Id = app.Id,
            Name = app.Name ?? string.Empty,
            Domain = app.Domain ?? string.Empty,
            DefaultTheme = app.DefaultTheme ?? string.Empty,
            DefaultCulture = app.DefaultCultureId ?? string.Empty,
            Config = app.Config,
            TemplatesByName = (app.Templates ?? new List<Template>())
                .GroupBy(keySelector: template => template.Name ?? string.Empty, comparer: StringComparer.OrdinalIgnoreCase)
        .ToDictionary(keySelector: group => group.Key, elementSelector: group => MapTemplate(template: group.First()), comparer: StringComparer.OrdinalIgnoreCase),
            PagesById = (app.Pages ?? new List<Page>())
                .GroupBy(keySelector: foundPage => foundPage.Id)
        .ToDictionary(keySelector: group => group.Key, elementSelector: group => MapPage(page: group.First(), culture: culture, includeContent: false))
        };

    private static PageRenderTemplate MapTemplate(Template template) =>
        new PageRenderTemplate
        {
            Name = template.Name ?? string.Empty,
            ResourceKey = template.ResourceKey ?? string.Empty,
            RawString = template.RawString ?? string.Empty
        };

    private static PageRenderPage MapPage(Page page, string culture, bool includeContent) =>
        new PageRenderPage
        {
            Id = page.Id,
            ParentId = page.ParentId,
            AppId = page.AppId,
            Order = page.Order,
            ShowOnMenus = page.ShowOnMenus,
            Path = page.Path ?? string.Empty,
            Name = page.Name ?? string.Empty,
            ResourceKey = page.ResourceKey ?? string.Empty,
            LayoutName = page.Layout ?? string.Empty,
            Title = GetPageInfo(page: page, culture: culture).Title,
            Description = GetPageInfo(page: page, culture: culture).Description,
            Keywords = GetPageInfo(page: page, culture: culture).Keywords,
            ContentByName = includeContent
                ? BuildContentLookup(contents: page.Contents, culture: culture)
                : new Dictionary<string, PageRenderContent>(comparer: StringComparer.OrdinalIgnoreCase)
        };

    private static PageRenderUser MapUser(User user) =>
        new PageRenderUser
        {
            Id = user.Id ?? string.Empty,
            DefaultCultureId = user.DefaultCultureId ?? string.Empty,
            DisplayName = user.DisplayName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            AppPrivileges = (user.Roles ?? new List<UserRole>())
                .Where(predicate: role => role.Role?.AppId != null)
        .GroupBy(keySelector: role => role.Role.AppId)
        .ToDictionary(
keySelector: group => group.Key,
elementSelector: group => (ISet<string>)new HashSet<string>(
collection: group.SelectMany(selector: role => role.Role?.Privileges ?? new List<string>()),
comparer: StringComparer.OrdinalIgnoreCase))
        };

    private static PageRenderLayout ResolveLayout(App app, string layoutName)
    {
        Layout layout = app.Layouts?.FirstOrDefault(predicate: item => item.Name == layoutName)
            ?? app.Layouts?.FirstOrDefault();

        return layout == null
            ? new PageRenderLayout
            {
                Name = string.Empty,
                HeaderHtml = string.Empty,
                BodyHtml = "[content[body]]"
            }
            : new PageRenderLayout
            {
                Name = layout.Name ?? string.Empty,
                HeaderHtml = layout.HeaderHtml ?? string.Empty,
                BodyHtml = layout.Html ?? "[content[body]]"
            };
    }

    private static IReadOnlyDictionary<string, PageRenderContent> BuildContentLookup(IEnumerable<Content> contents, string culture) =>
        (contents ?? Array.Empty<Content>())
            .GroupBy(keySelector: content => content.Name ?? string.Empty, comparer: StringComparer.OrdinalIgnoreCase)
        .ToDictionary(
keySelector: group => group.Key,
elementSelector: group => MapContent(content: GetClosestContent(potentials: group, culture: culture) ?? group.First()),
comparer: StringComparer.OrdinalIgnoreCase);

    private static Content GetClosestContent(IEnumerable<Content> potentials, string culture)
    {
        Content content = null;

        List<string> cultureParts = (culture ?? string.Empty).ToLowerInvariant()
            .Split(separator: '-')
            .ToList();

        int count = cultureParts.Count;
        string resultCulture = string.Empty;

        while (content == null && resultCulture != null)
        {
            resultCulture = string.Join(separator: "-", values: cultureParts.Take(count: count));

            content = potentials.FirstOrDefault(predicate: candidate =>
                string.Equals(a: candidate.CultureId ?? string.Empty, b: resultCulture ?? string.Empty, comparisonType: StringComparison.OrdinalIgnoreCase));

            count--;

            if (count == 0)
            {
                resultCulture = null;
            }
        }

        return content ?? potentials.FirstOrDefault(predicate: candidate => string.IsNullOrEmpty(value: candidate.CultureId));
    }

    private static PageRenderContent MapContent(Content content) =>
        new PageRenderContent
        {
            Id = content.Id,
            Name = content.Name ?? string.Empty,
            Html = content.Html ?? string.Empty
        };

    private static IReadOnlyList<PageRenderResource> MapResources(IEnumerable<Resource> resources) =>
        (resources ?? Array.Empty<Resource>())
            .Select(selector: resource => new PageRenderResource
            {
                Key = resource.Key ?? string.Empty,
                Culture = resource.Culture ?? string.Empty,
                Name = resource.Name ?? string.Empty,
                DisplayName = resource.DisplayName ?? resource.Name ?? string.Empty,
                ShortDisplayName = resource.ShortDisplayName ?? resource.Name ?? string.Empty,
                Description = resource.Description ?? string.Empty
            })
        .ToArray();

    private static IReadOnlyDictionary<string, PageRenderResource> BuildResourceLookup(IEnumerable<Resource> resources) =>
        (resources ?? Array.Empty<Resource>())
            .GroupBy(keySelector: resource => $"{resource.Key ?? string.Empty}|{resource.Name ?? string.Empty}|{resource.Culture ?? string.Empty}", comparer: StringComparer.OrdinalIgnoreCase)
        .ToDictionary(
keySelector: group => group.Key,
elementSelector: group => new PageRenderResource
{
    Key = group.First()
        .Key ?? string.Empty,
    Culture = group.First()
        .Culture ?? string.Empty,
    Name = group.First()
        .Name ?? string.Empty,
    DisplayName = group.First()
        .DisplayName ?? group.First()
        .Name ?? string.Empty,
    ShortDisplayName = group.First()
        .ShortDisplayName ?? group.First()
        .Name ?? string.Empty,
    Description = group.First()
        .Description ?? string.Empty
},
comparer: StringComparer.OrdinalIgnoreCase);

    private static IDictionary<string, PageRenderComponent> BuildComponentLookup(IEnumerable<Component> components) =>
        (components ?? Array.Empty<Component>())
            .GroupBy(keySelector: component => component.Name ?? string.Empty, comparer: StringComparer.OrdinalIgnoreCase)
        .ToDictionary(
keySelector: group => group.Key,
elementSelector: group => new PageRenderComponent
{
    Id = group.First()
        .Id,
    Name = group.First()
        .Name ?? string.Empty,
    ResourceKey = group.First()
        .ResourceKey ?? string.Empty,
    Content = group.First()
        .Content ?? string.Empty,
    Script = group.First()
        .Script ?? string.Empty
},
comparer: StringComparer.OrdinalIgnoreCase);

    private static IDictionary<string, PageRenderScript> BuildScriptLookup(IEnumerable<Script> scripts) =>
        (scripts ?? Array.Empty<Script>())
            .GroupBy(keySelector: script => script.Name ?? string.Empty, comparer: StringComparer.OrdinalIgnoreCase)
        .ToDictionary(
keySelector: group => group.Key,
elementSelector: group => new PageRenderScript
{
    Name = group.First()
        .Name ?? string.Empty,
    Content = group.First()
        .Content ?? string.Empty
},
comparer: StringComparer.OrdinalIgnoreCase);

    private static void ValidatePage(Page page, string parameterName) =>
        ThrowIf(condition: page == null, message: parameterName + " is required.");

    private static void ValidateUser(User user, string parameterName) =>
        ThrowIf(condition: user == null, message: parameterName + " is required.");

    private static void ValidateTheme(string theme, string parameterName) =>
        ThrowIf(condition: string.IsNullOrWhiteSpace(value: theme), message: parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }

    private static PageInfo GetPageInfo(Page page, string culture)
    {
        culture ??= string.Empty;

        if (page?.PageInfo == null || !page.PageInfo.Any())
        {
            return new PageInfo
            {
                CultureId = culture,
                Title = page?.Name ?? string.Empty,
                Description = string.Empty,
                Keywords = string.Empty
            };
        }

        IOrderedEnumerable<PageInfo> orderedInfo = page.PageInfo
            .OrderByDescending(
                keySelector: info => info.CultureId?.Length ?? 0);

        return orderedInfo.FirstOrDefault(
            predicate: info =>
                culture == info.CultureId
                || culture.Contains(value: info.CultureId ?? string.Empty))
            ?? orderedInfo.FirstOrDefault()
            ?? new PageInfo
            {
                CultureId = culture,
                Title = page.Name ?? string.Empty,
                Description = string.Empty,
                Keywords = string.Empty
            };
    }
}