// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Extensions;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Models.PageRendering;
using cCoder.ContentManagement.Services;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

using cCoder.ContentManagement.Services.Foundations;
using cCoder.ContentManagement.Models.Rendering;
using cCoder.ContentManagement.Services.Foundations.Rendering;

namespace cCoder.ContentManagement.Services.Processings;

internal sealed partial class PageRenderProcessingService(
    IPageRenderService pageRenderService,
    ContentManagementConfiguration config)
        : IPageRenderProcessingService
{
    public string ComputeFingerprint(object value) =>
        TryCatch<string>(operation: () =>
        {
            ValidateSerializeRuntimeValue(inputs: [value]);

            return pageRenderService.ComputeFingerprintPageRenderFoundationOperation(
                pageRenderFoundationOperation: new PageRenderFoundationOperation
                {
                    Value = value
                })
            .Json;
        });

    public string SerializeRuntimeValue(object value) =>
        TryCatch<string>(operation: () =>
        {
            ValidateSerializeRuntimeValue(inputs: [value]);

            return pageRenderService.SerializePageRenderFoundationOperation(
                pageRenderFoundationOperation: new PageRenderFoundationOperation
                {
                    Value = value
                })
            .Json;
        });

    public PageRenderOperation RenderPageRenderOperation(
        PageRenderOperation pageRenderOperation) =>
        TryCatch<PageRenderOperation>(operation: () =>
    {
        ValidateRenderPageRenderOperation(inputs: [pageRenderOperation]);

        ValidatePage(
            page: pageRenderOperation.SourcePage,
            parameterName: "pageRenderOperation.SourcePage");

        ValidateUser(
            user: pageRenderOperation.User,
            parameterName: "pageRenderOperation.User");

        ValidateTheme(
            theme: pageRenderOperation.Theme,
            parameterName: "pageRenderOperation.Theme");

        pageRenderOperation.RenderSession = BuildSession(
            page: pageRenderOperation.SourcePage,
            user: pageRenderOperation.User,
            config: config,
            theme: pageRenderOperation.Theme,
            culture: pageRenderOperation.Culture,
            edit: pageRenderOperation.Edit,
            headerOnly: pageRenderOperation.HeaderOnly,
            cacheTemplate: pageRenderOperation.CacheTemplate);

        return pageRenderOperation;
    });

    public PageRenderOperation CompletePageRenderOperation(
        PageRenderOperation pageRenderOperation) =>
        TryCatch<PageRenderOperation>(operation: () =>
    {
        ValidateRenderPageRenderOperation(inputs: [pageRenderOperation]);

        RenderSession renderedSession = pageRenderOperation.RenderSession;

        pageRenderOperation.Page = new PageRenderResult
        {
            AppId = renderedSession.App?.Id ?? 0,
            PageId = renderedSession.Page?.Id ?? 0,
            ParentId = renderedSession.Page?.ParentId,
            Theme = renderedSession.Request.Theme ?? string.Empty,
            Culture = renderedSession.Request.Culture ?? string.Empty,
            Edit = renderedSession.Request.Edit,
            Path = renderedSession.Page?.Path
                ?? renderedSession.Request.Path
                ?? string.Empty,
            Layout = renderedSession.Layout?.Name
                ?? renderedSession.Page?.LayoutName
                ?? string.Empty,
            Title = renderedSession.Page?.Title ?? string.Empty,
            Description = renderedSession.Page?.Description ?? string.Empty,
            Keywords = renderedSession.Page?.Keywords ?? string.Empty,
            HeaderHtml = renderedSession.Output?.HeaderMarkup ?? string.Empty,
            BodyHtml = renderedSession.Output?.BodyMarkup ?? string.Empty,
            StatusCode = renderedSession.Page == null ? 404 : 200
        };

        return pageRenderOperation;
    });

    private static RenderSession BuildSession(
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

        PageRenderLayout layout = ResolveLayout(
            app: app,
            layoutName: page.Layout);

        return new RenderSession
        {
            Request = new RenderRequest
            {
                AppId = app.Id,
                Path = page.Path ?? string.Empty,
                Theme = resolvedTheme,
                Culture = resolvedCulture,
                Edit = edit,
                HeaderOnly = headerOnly,
                CacheTemplate = cacheTemplate
            },
            Target = new RenderTarget
            {
                Scope = RenderScope.Page,
                ResourceKey = page.ResourceKey ?? "Default",
                HeaderMarkup = layout.HeaderHtml ?? string.Empty,
                BodyMarkup = layout.BodyHtml ?? string.Empty,
                AllowHeaderContentTags = false,
                AllowBodyContentTags = true
            },
            Config = config,
            App = MapApp(app: app, culture: resolvedCulture),
            Page = MapPage(page: page, culture: resolvedCulture, includeContent: true),
            User = MapUser(user: user),
            Layout = layout,
            Resources = MapResources(resources: app.Resources),
            ResourcesByLookup = BuildResourceLookup(resources: app.Resources),
            ComponentsByName = BuildComponentLookup(components: app.Components),
            ScriptsByName = BuildScriptLookup(scripts: app.Scripts),
            EmittedScriptNames = new HashSet<string>(comparer: StringComparer.OrdinalIgnoreCase)
        };
    }

    private static PageRenderApp MapApp(App app, string culture)
    {
        Dictionary<string, PageRenderTemplate> templatesByName =
            new(comparer: StringComparer.OrdinalIgnoreCase);

        foreach (Template template in app.Templates ?? [])
        {
            string name = template.Name ?? string.Empty;

            if (!templatesByName.ContainsKey(key: name))
            {
                templatesByName.Add(key: name, value: MapTemplate(template: template));
            }
        }

        Dictionary<int, PageRenderPage> pagesById = [];

        foreach (Page page in app.Pages ?? [])
        {
            if (!pagesById.ContainsKey(key: page.Id))
            {
                pagesById.Add(
                    key: page.Id,
                    value: MapPage(page: page, culture: culture, includeContent: false));
            }
        }

        return new PageRenderApp
        {
            Id = app.Id,
            Name = app.Name ?? string.Empty,
            Domain = app.Domain ?? string.Empty,
            DefaultTheme = app.DefaultTheme ?? string.Empty,
            DefaultCulture = app.DefaultCultureId ?? string.Empty,
            Config = app.Config,
            TemplatesByName = templatesByName,
            PagesById = pagesById
        };
    }

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

    private static PageRenderUser MapUser(User user)
    {
        Dictionary<int, ISet<string>> appPrivileges = [];

        foreach (UserRole userRole in user.Roles ?? [])
        {
            if (userRole.Role?.AppId is not int appId)
            {
                continue;
            }

            if (!appPrivileges.TryGetValue(key: appId, value: out ISet<string> privileges))
            {
                privileges = new HashSet<string>(comparer: StringComparer.OrdinalIgnoreCase);
                appPrivileges.Add(key: appId, value: privileges);
            }

            foreach (string privilege in userRole.Role.Privileges ?? [])
            {
                privileges.Add(item: privilege);
            }
        }

        return new PageRenderUser
        {
            Id = user.Id ?? string.Empty,
            DefaultCultureId = user.DefaultCultureId ?? string.Empty,
            DisplayName = user.DisplayName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            AppPrivileges = appPrivileges
        };
    }

    private static PageRenderLayout ResolveLayout(App app, string layoutName)
    {
        Layout layout = null;
        Layout firstLayout = null;

        foreach (Layout candidate in app.Layouts ?? [])
        {
            firstLayout ??= candidate;

            if (candidate.Name == layoutName)
            {
                layout = candidate;
                break;
            }
        }

        layout ??= firstLayout;

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

    private static IReadOnlyDictionary<string, PageRenderContent> BuildContentLookup(
        IEnumerable<Content> contents,
        string culture)
    {
        Dictionary<string, List<Content>> contentsByName =
            new(comparer: StringComparer.OrdinalIgnoreCase);

        foreach (Content content in contents ?? [])
        {
            string name = content.Name ?? string.Empty;

            if (!contentsByName.TryGetValue(key: name, value: out List<Content> potentials))
            {
                potentials = [];
                contentsByName.Add(key: name, value: potentials);
            }

            potentials.Add(item: content);
        }

        Dictionary<string, PageRenderContent> contentLookup =
            new(comparer: StringComparer.OrdinalIgnoreCase);

        foreach (KeyValuePair<string, List<Content>> entry in contentsByName)
        {
            Content content = GetClosestContent(
                potentials: entry.Value,
                culture: culture) ?? entry.Value[0];

            contentLookup.Add(key: entry.Key, value: MapContent(content: content));
        }

        return contentLookup;
    }

    private static Content GetClosestContent(IEnumerable<Content> potentials, string culture)
    {
        Content content = null;

        string[] cultureParts = (culture ?? string.Empty).ToLowerInvariant()
            .Split(separator: '-');

        int count = cultureParts.Length;
        string resultCulture = string.Empty;

        while (content == null && resultCulture != null)
        {
            resultCulture = string.Join(separator: "-", value: cultureParts, startIndex: 0, count: count);
            content = FindContentByCulture(potentials: potentials, culture: resultCulture);

            count--;

            if (count == 0)
            {
                resultCulture = null;
            }
        }

        return content ?? FindContentByCulture(potentials: potentials, culture: string.Empty);
    }

    private static Content FindContentByCulture(IEnumerable<Content> potentials, string culture)
    {
        foreach (Content candidate in potentials ?? [])
        {
            if (string.Equals(
                a: candidate.CultureId ?? string.Empty,
                b: culture ?? string.Empty,
                comparisonType: StringComparison.OrdinalIgnoreCase))
            {
                return candidate;
            }
        }

        return null;
    }

    private static PageRenderContent MapContent(Content content) =>
        new PageRenderContent
        {
            Id = content.Id,
            Name = content.Name ?? string.Empty,
            Html = content.Html ?? string.Empty
        };

    private static IReadOnlyList<PageRenderResource> MapResources(IEnumerable<Resource> resources)
    {
        List<PageRenderResource> mappedResources = [];

        foreach (Resource resource in resources ?? [])
        {
            mappedResources.Add(item: new PageRenderResource
            {
                Key = resource.Key ?? string.Empty,
                Culture = resource.Culture ?? string.Empty,
                Name = resource.Name ?? string.Empty,
                DisplayName = resource.DisplayName ?? resource.Name ?? string.Empty,
                ShortDisplayName = resource.ShortDisplayName ?? resource.Name ?? string.Empty,
                Description = resource.Description ?? string.Empty
            });
        }

        return mappedResources;
    }

    private static IReadOnlyDictionary<string, PageRenderResource> BuildResourceLookup(
        IEnumerable<Resource> resources)
    {
        Dictionary<string, PageRenderResource> lookup =
            new(comparer: StringComparer.OrdinalIgnoreCase);

        foreach (Resource resource in resources ?? [])
        {
            string key = $"{resource.Key ?? string.Empty}|{resource.Name ?? string.Empty}|{resource.Culture ?? string.Empty}";

            if (!lookup.ContainsKey(key: key))
            {
                lookup.Add(key: key, value: new PageRenderResource
                {
                    Key = resource.Key ?? string.Empty,
                    Culture = resource.Culture ?? string.Empty,
                    Name = resource.Name ?? string.Empty,
                    DisplayName = resource.DisplayName ?? resource.Name ?? string.Empty,
                    ShortDisplayName = resource.ShortDisplayName ?? resource.Name ?? string.Empty,
                    Description = resource.Description ?? string.Empty
                });
            }
        }

        return lookup;
    }

    private static IDictionary<string, PageRenderComponent> BuildComponentLookup(
        IEnumerable<Component> components)
    {
        Dictionary<string, PageRenderComponent> lookup =
            new(comparer: StringComparer.OrdinalIgnoreCase);

        foreach (Component component in components ?? [])
        {
            string name = component.Name ?? string.Empty;

            if (!lookup.ContainsKey(key: name))
            {
                lookup.Add(key: name, value: new PageRenderComponent
                {
                    Id = component.Id,
                    Name = name,
                    ResourceKey = component.ResourceKey ?? string.Empty,
                    Content = component.Content ?? string.Empty,
                    Script = component.Script ?? string.Empty
                });
            }
        }

        return lookup;
    }

    private static IDictionary<string, PageRenderScript> BuildScriptLookup(
        IEnumerable<Script> scripts)
    {
        Dictionary<string, PageRenderScript> lookup =
            new(comparer: StringComparer.OrdinalIgnoreCase);

        foreach (Script script in scripts ?? [])
        {
            string name = script.Name ?? string.Empty;

            if (!lookup.ContainsKey(key: name))
            {
                lookup.Add(key: name, value: new PageRenderScript
                {
                    Name = name,
                    Content = script.Content ?? string.Empty
                });
            }
        }

        return lookup;
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

    private static PageInfo GetPageInfo(Page page, string culture)
    {
        culture ??= string.Empty;

        if (page?.PageInfo == null || page.PageInfo.Count == 0)
        {
            return new PageInfo
            {
                CultureId = culture,
                Title = page?.Name ?? string.Empty,
                Description = string.Empty,
                Keywords = string.Empty
            };
        }

        PageInfo bestMatch = null;
        PageInfo longest = null;

        foreach (PageInfo info in page.PageInfo)
        {
            if (longest == null
                || (info.CultureId?.Length ?? 0) > (longest.CultureId?.Length ?? 0))
            {
                longest = info;
            }

            if ((culture == info.CultureId
                    || culture.Contains(value: info.CultureId ?? string.Empty))
                && (bestMatch == null
                    || (info.CultureId?.Length ?? 0) > (bestMatch.CultureId?.Length ?? 0)))
            {
                bestMatch = info;
            }
        }

        return bestMatch
            ?? longest
            ?? new PageInfo
            {
                CultureId = culture,
                Title = page.Name ?? string.Empty,
                Description = string.Empty,
                Keywords = string.Empty
            };
    }
}