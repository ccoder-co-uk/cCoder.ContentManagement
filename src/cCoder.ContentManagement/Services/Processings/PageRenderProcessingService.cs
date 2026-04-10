using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Rendering.Models;
using cCoder.ContentManagement.Rendering.Services.Orchestrations;
using cCoder.ContentManagement.Services;
using App = cCoder.Data.Models.CMS.App;
using Component = cCoder.Data.Models.CMS.Component;
using Content = cCoder.Data.Models.CMS.Content;
using Layout = cCoder.Data.Models.CMS.Layout;
using Page = cCoder.Data.Models.CMS.Page;
using Resource = cCoder.Data.Models.CMS.Resource;
using Script = cCoder.Data.Models.CMS.Script;
using Template = cCoder.Data.Models.CMS.Template;
using User = cCoder.Data.Models.Security.User;
using UserRole = cCoder.Data.Models.Security.UserRole;
using RenderResult = cCoder.ContentManagement.Models.RenderResult;

namespace cCoder.ContentManagement.Services.Processings;

internal sealed class PageRenderProcessingService(
    IPageRenderExecutionOrchestrationService executionOrchestrationService) : IPageRenderProcessingService
{
    public RenderResult RenderPage(Page page, User user, Config config, string theme, string culture, bool edit = false)
    {
        ValidatePage(page, "page");
        ValidateUser(user, "user");
        ValidateTheme(theme, "theme");

        PageRenderSession session = BuildSession(page, user, config, theme, culture, edit);
        PageRenderResult pageRenderResult = executionOrchestrationService.Render(session);

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
        App app = page.App ?? throw new InvalidOperationException("page.App is required.");
        string resolvedTheme = string.IsNullOrWhiteSpace(theme) ? app.DefaultTheme ?? "Default" : theme;
        string resolvedCulture = string.IsNullOrWhiteSpace(culture)
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
            App = MapApp(app, resolvedCulture),
            Page = MapPage(page, resolvedCulture, includeContent: true),
            User = MapUser(user),
            Layout = ResolveLayout(app, page.Layout),
            Resources = MapResources(app.Resources),
            ResourcesByLookup = BuildResourceLookup(app.Resources),
            ComponentsByName = BuildComponentLookup(app.Components),
            ScriptsByName = BuildScriptLookup(app.Scripts)
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
                .GroupBy(template => template.Name ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(group => group.Key, group => MapTemplate(group.First()), StringComparer.OrdinalIgnoreCase),
            PagesById = (app.Pages ?? new List<Page>())
                .GroupBy(foundPage => foundPage.Id)
                .ToDictionary(group => group.Key, group => MapPage(group.First(), culture, includeContent: false))
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
            Title = ContentManagementModelLogic.Title(page, culture),
            Description = ContentManagementModelLogic.Description(page, culture),
            Keywords = ContentManagementModelLogic.Keywords(page, culture),
            ContentByName = includeContent
                ? BuildContentLookup(page.Contents, culture)
                : new Dictionary<string, PageRenderContent>(StringComparer.OrdinalIgnoreCase)
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
                .Where(role => role.Role?.AppId != null)
                .GroupBy(role => role.Role.AppId)
                .ToDictionary(
                    group => group.Key,
                    group => (ISet<string>)new HashSet<string>(
                        group.SelectMany(role => role.Role?.Privileges ?? new List<string>()),
                        StringComparer.OrdinalIgnoreCase))
        };
    }

    private static PageRenderLayout ResolveLayout(App app, string layoutName)
    {
        Layout layout = app.Layouts?.FirstOrDefault(item => item.Name == layoutName)
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
            .GroupBy(content => content.Name ?? string.Empty, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                group => group.Key,
                group => MapContent(GetClosestContent(group, culture) ?? group.First()),
                StringComparer.OrdinalIgnoreCase);
    }

    private static Content GetClosestContent(IEnumerable<Content> potentials, string culture)
    {
        Content content = null;
        List<string> cultureParts = (culture ?? string.Empty).ToLowerInvariant().Split('-').ToList();
        int count = cultureParts.Count;
        string resultCulture = string.Empty;

        while (content == null && resultCulture != null)
        {
            resultCulture = string.Join("-", cultureParts.Take(count));
            content = potentials.FirstOrDefault(candidate =>
                string.Equals(candidate.CultureId ?? string.Empty, resultCulture ?? string.Empty, StringComparison.OrdinalIgnoreCase));
            count--;

            if (count == 0)
            {
                resultCulture = null;
            }
        }

        return content ?? potentials.FirstOrDefault(candidate => string.IsNullOrEmpty(candidate.CultureId));
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
            .Select(resource => new PageRenderResource
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
            .GroupBy(resource => $"{resource.Key ?? string.Empty}|{resource.Name ?? string.Empty}|{resource.Culture ?? string.Empty}", StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                group => group.Key,
                group => new PageRenderResource
                {
                    Key = group.First().Key ?? string.Empty,
                    Culture = group.First().Culture ?? string.Empty,
                    Name = group.First().Name ?? string.Empty,
                    DisplayName = group.First().DisplayName ?? group.First().Name ?? string.Empty,
                    ShortDisplayName = group.First().ShortDisplayName ?? group.First().Name ?? string.Empty,
                    Description = group.First().Description ?? string.Empty
                },
                StringComparer.OrdinalIgnoreCase);
    }

    private static IDictionary<string, PageRenderComponent> BuildComponentLookup(IEnumerable<Component> components)
    {
        return (components ?? Array.Empty<Component>())
            .GroupBy(component => component.Name ?? string.Empty, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                group => group.Key,
                group => new PageRenderComponent
                {
                    Id = group.First().Id,
                    Name = group.First().Name ?? string.Empty,
                    ResourceKey = group.First().ResourceKey ?? string.Empty,
                    Content = group.First().Content ?? string.Empty,
                    Script = group.First().Script ?? string.Empty
                },
                StringComparer.OrdinalIgnoreCase);
    }

    private static IDictionary<string, PageRenderScript> BuildScriptLookup(IEnumerable<Script> scripts)
    {
        return (scripts ?? Array.Empty<Script>())
            .GroupBy(script => script.Name ?? string.Empty, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                group => group.Key,
                group => new PageRenderScript
                {
                    Name = group.First().Name ?? string.Empty,
                    Content = group.First().Content ?? string.Empty
                },
                StringComparer.OrdinalIgnoreCase);
    }

    private static void ValidatePage(Page page, string parameterName)
    {
        if (page == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
    }

    private static void ValidateUser(User user, string parameterName)
    {
        if (user == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
    }

    private static void ValidateTheme(string theme, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(theme))
        {
            throw new ValidationException(parameterName + " is required.");
        }
    }
}
