// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Rendering.Models;
using cCoder.ContentManagement.Rendering.Services.Orchestrations;
using cCoder.ContentManagement.Services;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Processings;

internal sealed class PageRenderProcessingService(
    IPageRenderExecutionOrchestrationService executionOrchestrationService) : IPageRenderProcessingService
{
    public RenderResult RenderPageUserConfigRenderResult(Page page, User user, Config config, string theme, string culture, bool edit = false)
    {
        ValidatePage(page: page, parameterName: "page");
        ValidateUser(user: user, parameterName: "user");
        ValidateTheme(theme: theme, parameterName: "theme");

        PageRenderSession session = BuildSession(page: page, user: user, config: config, theme: theme, culture: culture, edit: edit);
        PageRenderResult pageRenderResult = executionOrchestrationService.RenderPageRenderSessionPageRenderResult(session: session);

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
    }

    private static PageRenderSession BuildSession(Page page, User user, Config config, string theme, string culture, bool edit)
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
                Edit = edit
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

    private static PageRenderApp MapApp(App app, string culture)
    {
        return new PageRenderApp
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
    }

    private static PageRenderTemplate MapTemplate(Template template)
    {
        return new PageRenderTemplate
        {
            Name = template.Name ?? string.Empty,
            ResourceKey = template.ResourceKey ?? string.Empty,
            RawString = template.RawString ?? string.Empty
        };
    }

    private static PageRenderPage MapPage(Page page, string culture, bool includeContent)
    {
        return new PageRenderPage
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
            Title = ContentManagementModelLogic.Title(page: page, culture: culture),
            Description = ContentManagementModelLogic.Description(page: page, culture: culture),
            Keywords = ContentManagementModelLogic.Keywords(page: page, culture: culture),
            ContentByName = includeContent
                ? BuildContentLookup(contents: page.Contents, culture: culture)
                : new Dictionary<string, PageRenderContent>(comparer: StringComparer.OrdinalIgnoreCase)
        };
    }

    private static PageRenderUser MapUser(User user)
    {
        return new PageRenderUser
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
    }

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

    private static IReadOnlyDictionary<string, PageRenderContent> BuildContentLookup(IEnumerable<Content> contents, string culture)
    {
        return (contents ?? Array.Empty<Content>())
            .GroupBy(keySelector: content => content.Name ?? string.Empty, comparer: StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
keySelector: group => group.Key,
elementSelector: group => MapContent(content: GetClosestContent(potentials: group, culture: culture) ?? group.First()),
comparer: StringComparer.OrdinalIgnoreCase);
    }

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

    private static PageRenderContent MapContent(Content content)
    {
        return new PageRenderContent
        {
            Id = content.Id,
            Name = content.Name ?? string.Empty,
            Html = content.Html ?? string.Empty
        };
    }

    private static IReadOnlyList<PageRenderResource> MapResources(IEnumerable<Resource> resources)
    {
        return (resources ?? Array.Empty<Resource>())
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
    }

    private static IReadOnlyDictionary<string, PageRenderResource> BuildResourceLookup(IEnumerable<Resource> resources)
    {
        return (resources ?? Array.Empty<Resource>())
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
    }

    private static IDictionary<string, PageRenderComponent> BuildComponentLookup(IEnumerable<Component> components)
    {
        return (components ?? Array.Empty<Component>())
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
    }

    private static IDictionary<string, PageRenderScript> BuildScriptLookup(IEnumerable<Script> scripts)
    {
        return (scripts ?? Array.Empty<Script>())
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
    }

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
}