using System.Collections;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Rendering.Brokers;
using cCoder.ContentManagement.Rendering.Models;
using cCoder.ContentManagement.Services.Foundations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace cCoder.ContentManagement.Rendering.Services.Foundations;

internal sealed class MarkupRenderService(
    IComponentReaderBroker componentReaderBroker,
    IScriptReaderBroker scriptReaderBroker,
    IJsonBroker jsonBroker,
    IRenderFileContentService renderFileContentService) : IMarkupRenderService
{
    private static readonly RegexOptions regexOptions = RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline;

    private static readonly PageRenderSyntax syntax = new()
    {
        ContentRegex = new Regex("\\[content\\[(?<name>[A-Za-z\\d_\\-/. ]+)\\](?<options>[^\\]]*)\\]", regexOptions),
        ResourceDisplayNameRegex = new Regex("\\[resource_displayname\\[(?<name>[A-Za-z\\d_\\-/. ]+)\\]\\]", regexOptions),
        ResourceShortDisplayNameRegex = new Regex("\\[resource_shortdisplayname\\[(?<name>[A-Za-z\\d_\\-/. ]+)\\]\\]", regexOptions),
        ResourceDescriptionRegex = new Regex("\\[resource_description\\[(?<name>[A-Za-z\\d_\\-/. ]+)\\]\\]", regexOptions),
        NavRegex = new Regex("\\[nav\\[(?<name>[A-Za-z\\d_\\-/. ]*)\\]\\]", regexOptions),
        NavExpandedRegex = new Regex("\\[navExpanded\\[(?<name>[A-Za-z\\d_\\-/. ]*)\\]\\]", regexOptions),
        DmsRegex = new Regex("\\[dms\\[(?<name>[A-Za-z\\d_\\-/. ]+)\\]\\]", regexOptions),
        ExecuteRegex = new Regex("\\[execute\\](.*?)\\[/execute\\]", regexOptions),
        ComponentRegex = new Regex("\\[component\\[(?<name>[A-Za-z\\d_\\-/. ]+)\\](?<options>[^\\]]*)\\]", regexOptions),
        ScriptRegex = new Regex("\\[script\\[(?<name>[A-Za-z\\d_\\-/. ]+)\\]\\]", regexOptions),
        MetaRegex = new Regex("\\[meta\\[(?<name>[A-Za-z\\d_\\-/. ]+)\\]\\]", regexOptions),
        CultureLinkRegex = new Regex("\\[culturelink\\[(?<name>[A-Za-z\\d_\\-/. ]+)\\]\\]", regexOptions)
    };

    public PageRenderResult Render(PageRenderSession session)
    {
        string key = string.IsNullOrWhiteSpace(session.Page?.ResourceKey) ? "Default" : session.Page.ResourceKey;
        string culture = ResolveCulture(session);
        List<Replacement> replacements = BuildDefaultReplacements(session).ToList();
        AddThemeTemplateReplacements(session, replacements);

        return new PageRenderResult
        {
            AppId = session.App?.Id ?? 0,
            PageId = session.Page?.Id ?? 0,
            ParentId = session.Page?.ParentId,
            Theme = session.Request.Theme ?? string.Empty,
            Culture = culture,
            Edit = session.Request.Edit,
            Path = session.Page?.Path ?? session.Request.Path ?? string.Empty,
            Layout = session.Layout?.Name ?? session.Page?.LayoutName ?? string.Empty,
            Title = session.Page?.Title ?? string.Empty,
            Description = session.Page?.Description ?? string.Empty,
            Keywords = session.Page?.Keywords ?? string.Empty,
            HeaderHtml = RenderMarkup(key, session.Layout?.HeaderHtml ?? string.Empty, session, replacements, allowContentTags: false),
            BodyHtml = RenderMarkup(key, session.Layout?.BodyHtml ?? string.Empty, session, replacements),
            StatusCode = session.Page == null ? 404 : 200
        };
    }

    private string RenderMarkup(string key, string content, PageRenderSession session, IReadOnlyCollection<Replacement> replacements, bool allowContentTags = true)
    {
        if (string.IsNullOrEmpty(content))
        {
            return string.Empty;
        }

        StringBuilder result = new(content, content.Length * 4);

        if (allowContentTags)
        {
            Content(key, result, session, replacements);
        }

        Nav(result, session);
        Dms(key, result, session, replacements);
        Script(key, result, session, replacements);
        RegexReplace(result, syntax.CultureLinkRegex, static _ => "?culture=");
        Component(key, session, replacements, result);
        Meta(result, session);
        Resource(result, session, key, replacements);
        ExecuteAsync(key, result, session, replacements);

        foreach (Replacement replacement in replacements)
        {
            result.Replace(replacement.Old, replacement.New);
        }

        return result.ToString();
    }

    private void Content(string key, StringBuilder source, PageRenderSession session, IReadOnlyCollection<Replacement> replacements)
    {
        RegexReplace(source, syntax.ContentRegex, match =>
        {
            string name = GetName(match);
            string[] options = GetOptions(match);
            PageRenderContent pageRenderContent = null;

            if (session.Page != null && session.Page.ContentByName.TryGetValue(name, out PageRenderContent value))
            {
                pageRenderContent = value;
            }

            string optionalClass = string.Join(" ", options.Where(option => option.StartsWith("class=")).Select(option => option.Replace("class=", string.Empty)));
            string contentEditable = session.Request.Edit ? "contenteditable" : string.Empty;

            if (pageRenderContent == null)
            {
                return "[[Missing Content:" + name + "]]";
            }

            string html = $"<section name='{name}' class='content {optionalClass}' data-id='{pageRenderContent.Id}' {contentEditable} {string.Join(" ", options.Where(option => !option.StartsWith("class=")))}>\n                        {(session.Request.Edit ? pageRenderContent.Html : RenderMarkup(key, pageRenderContent.Html, session, replacements))}\n                    </section>";

            return session.Request.Edit
                ? html
                : RenderMarkup(key, html, session, replacements, allowContentTags: false);
        });
    }

    private void Nav(StringBuilder source, PageRenderSession session)
    {
        RegexReplace(source, syntax.NavRegex, match => BuildMenuFor(GetName(match), expand: false));
        RegexReplace(source, syntax.NavExpandedRegex, match => BuildMenuFor(GetName(match), expand: true));

        string BuildMenuFor(string tagName, bool expand)
        {
            PageRenderPage page = null;

            if (int.TryParse(tagName, out int pageId) && session.App != null)
            {
                session.App.PagesById.TryGetValue(pageId, out page);
            }

            return "<div class='collapse navbar-collapse'><ul class='navbar-nav'>" + BuildMenuItemsFor(page, expand) + "</ul></div>";
        }

        string BuildMenuItemsFor(PageRenderPage page, bool expand)
        {
            if (session.App == null)
            {
                return string.Empty;
            }

            return string.Join(
                "",
                session.App.PagesById.Values
                    .Where(subPage => subPage.ParentId == page?.Id && subPage.ShowOnMenus)
                    .OrderBy(subPage => subPage.Order)
                    .Select(subPage =>
                    {
                        string selected = subPage.ParentId.HasValue
                            && page != null
                            && !string.IsNullOrWhiteSpace(session.Page?.Path)
                            && session.Page.Path.Contains(subPage.Path)
                                ? " active"
                                : string.Empty;

                        return expand
                            ? $"<li data-id='{subPage.Id}' class='nav-item'><a href='/{subPage.Path}' class='nav-link{selected}'>{subPage.Title}</a><ul class='submenu dropdown-menu'>{BuildMenuItemsFor(subPage, expand: true)}</ul></li>"
                            : $"<li data-id='{subPage.Id}' class='nav-item'><a href='/{subPage.Path}' class='nav-link{selected}'>{subPage.Title}</a></li>";
                    }));
        }
    }

    private void Dms(string key, StringBuilder source, PageRenderSession session, IReadOnlyCollection<Replacement> replacements)
    {
        RegexReplace(source, syntax.DmsRegex, match =>
        {
            string name = GetName(match);
            string latestTextContent = renderFileContentService.GetLatestTextContent(session.App?.Id ?? 0, name);

            return string.IsNullOrEmpty(latestTextContent)
                ? string.Empty
                : RenderMarkup(key, latestTextContent, session, replacements, allowContentTags: false);
        });
    }

    private void Component(string key, PageRenderSession session, IReadOnlyCollection<Replacement> replacements, StringBuilder result)
    {
        if (session.Request.Edit)
        {
            return;
        }

        RegexReplace(result, syntax.ComponentRegex, match =>
        {
            string name = GetName(match);
            string[] options = GetOptions(match);
            PageRenderComponent component = ResolveComponent(session, name);

            if (component == null)
            {
                return "[[Missing Component:" + name + "]]";
            }

            string optionalClass = string.Join(" ", options.Where(option => option.StartsWith("class=")).Select(option => option.Replace("class=", string.Empty)));
            string content = $"<section name='{component.Name}' class='component {optionalClass}' data-id='{component.Id}' data-resource-key='{component.ResourceKey}' {string.Join(" ", options.Where(option => !option.StartsWith("class=")))}>\n                        {RenderMarkup(component.ResourceKey, component.Content, session, replacements, allowContentTags: false)}\n                        <script type='text/javascript'>{RenderMarkup(component.ResourceKey, component.Script, session, replacements, allowContentTags: false)}</script>\n                    </section>";

            return RenderMarkup(component.ResourceKey, content, session, replacements, allowContentTags: false);
        });
    }

    private void Script(string key, StringBuilder source, PageRenderSession session, IReadOnlyCollection<Replacement> replacements)
    {
        RegexReplace(source, syntax.ScriptRegex, match =>
        {
            string name = GetName(match);
            PageRenderScript script = ResolveScript(session, name);

            return script == null
                ? string.Empty
                : RenderMarkup(key, script.Content, session, replacements, allowContentTags: false);
        });
    }

    private void ExecuteAsync(string key, StringBuilder source, PageRenderSession session, IReadOnlyCollection<Replacement> replacements)
    {
        RegexReplace(source, syntax.ExecuteRegex, match =>
        {
            string code = match.Groups[1].Value;
            string json = replacements.FirstOrDefault(replacement => replacement.Old == "[model]")?.New ?? "{}";

            using HttpClient httpClient = new(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            })
            {
                BaseAddress = new Uri(replacements.First(replacement => replacement.Old == "[api[workflow]]").New),
                Timeout = TimeSpan.FromMinutes(10)
            };

            string content = SerializeForOData(new
            {
                Script = code,
                Model = jsonBroker.ParseJson(json)
            });

            Task<string> task = httpClient
                .PostAsync("ExecuteScript?useDetails=true", new StringContent(content, Encoding.UTF8, "text/plain"))
                .ContinueWith(responseTask => responseTask.Result.Content.ReadAsStringAsync())
                .Unwrap();

            task.Wait();

            return RenderMarkup(key, task.Result, session, replacements, allowContentTags: false);
        });
    }

    private static string SerializeForOData(object model)
    {
        return JsonConvert.SerializeObject(model, Formatting.None, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.None,
            Formatting = Formatting.None,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            NullValueHandling = NullValueHandling.Ignore,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            ContractResolver = new DefaultContractResolver
            {
                IgnoreSerializableAttribute = true
            },
            MaxDepth = 4
        });
    }

    private void Meta(StringBuilder source, PageRenderSession session)
    {
        RegexReplace(source, syntax.MetaRegex, match => session.MetadataResolver(GetName(match)) ?? string.Empty);
    }

    private void Resource(StringBuilder source, PageRenderSession session, string key, IReadOnlyCollection<Replacement> replacements)
    {
        if (session.Request.Edit)
        {
            return;
        }

        RegexReplace(source, syntax.ResourceDisplayNameRegex, match =>
            RenderMarkup(key, ResolveResource(session, key, GetName(match))?.DisplayName ?? GetName(match), session, replacements, allowContentTags: false));

        RegexReplace(source, syntax.ResourceShortDisplayNameRegex, match =>
            RenderMarkup(key, ResolveResource(session, key, GetName(match))?.ShortDisplayName ?? GetName(match), session, replacements, allowContentTags: false));

        RegexReplace(source, syntax.ResourceDescriptionRegex, match =>
            RenderMarkup(key, ResolveResource(session, key, GetName(match))?.Description ?? GetName(match), session, replacements, allowContentTags: false));
    }

    private IEnumerable<Replacement> BuildDefaultReplacements(PageRenderSession session)
    {
        string culture = ResolveCulture(session);
        string port = session.Config != null && session.Config.Settings.TryGetValue("sslPort", out string value) ? ":" + value : string.Empty;
        PageRenderUser user = session.User ?? new PageRenderUser();
        bool isGuest = string.IsNullOrWhiteSpace(user.Id) || string.Equals(user.Id, "Guest", StringComparison.OrdinalIgnoreCase);

        List<Replacement> replacements =
        [
            new("[[user]]", JsonConvert.SerializeObject(new
            {
                Id = isGuest ? "Guest" : user.Id,
                DefaultCultureId = string.IsNullOrWhiteSpace(user.DefaultCultureId) ? culture : user.DefaultCultureId,
                DisplayName = isGuest ? "Guest" : user.DisplayName,
                Email = user.Email ?? string.Empty
            })),
            new("[[displayname]]", isGuest ? "Guest" : user.DisplayName),
            new("[[loginlink]]", isGuest ? "<a href='/Login'>[resource_displayname[Login]]</a>" : "<a name='logout' href=''>[resource_displayname[Logout]]</a>"),
            new("[[date]]", DateTimeOffset.UtcNow.ToString("dd MMM yyyy")),
            new("[[culture]]", culture),
            new("[[lang]]", culture.Split('-').FirstOrDefault() ?? string.Empty),
            new("[app[name]]", session.App?.Name ?? string.Empty),
            new("[app[domain]]", session.App?.Domain ?? string.Empty),
            new("[app[root]]", "https://" + session.App?.Domain + port + "/"),
            new("[app[id]]", session.App?.Id.ToString() ?? string.Empty),
            new("[api[root]]", "https://" + session.App?.Domain + port + "/Api/"),
            new("[page[title]]", session.Page?.Title ?? string.Empty),
            new("[page[description]]", session.Page?.Description ?? string.Empty),
            new("[page[keywords]]", session.Page?.Keywords ?? string.Empty),
            new("[page[id]]", session.Page?.Id.ToString() ?? string.Empty),
            new("[page[parentid]]", session.Page?.ParentId?.ToString() ?? string.Empty),
            new("[page[path]]", session.Page?.Path ?? string.Empty),
            new("[page[url]]", string.IsNullOrWhiteSpace(session.Page?.Path) ? "https://" + session.App?.Domain + "/" : "https://" + session.App?.Domain + "/" + session.Page?.Path),
            new("[theme[name]]", session.Request.Theme ?? string.Empty),
            new("[[editlink]]", user.Can(session.App?.Id, "page_update") ? "<p style='cursor:pointer' onclick=\"setQueryParameter('edit', true)\">Edit</p>" : string.Empty)
        ];

        replacements.AddRange(BuildConfiguredReplacements(session));
        replacements.AddRange(BuildThemeValueReplacements(session));

        return replacements;
    }

    private static IEnumerable<Replacement> BuildConfiguredReplacements(PageRenderSession session)
    {
        if (session.Config != null
            && session.Config.Services.TryGetValue("Workflow", out string workflowService)
            && !string.IsNullOrWhiteSpace(workflowService))
        {
            yield return new Replacement("[api[workflow]]", workflowService);
        }
    }

    private IEnumerable<Replacement> BuildThemeValueReplacements(PageRenderSession session)
    {
        if (!TryGetThemeDictionary(session.App?.Config, out IDictionary<string, object> themeDictionary))
        {
            yield break;
        }

        object requestedTheme = null;

        if (!string.IsNullOrWhiteSpace(session.Request.Theme)
            && themeDictionary.TryGetValue(session.Request.Theme, out requestedTheme)
            && requestedTheme != null)
        {
            foreach (Replacement replacement in BuildThemeReplacements(requestedTheme))
            {
                yield return replacement;
            }

            yield break;
        }

        if (string.IsNullOrWhiteSpace(session.App?.DefaultTheme)
            || !themeDictionary.TryGetValue(session.App.DefaultTheme, out requestedTheme)
            || requestedTheme == null)
        {
            yield break;
        }

        foreach (Replacement replacement in BuildThemeReplacements(requestedTheme))
        {
            yield return replacement;
        }
    }

    private void AddThemeTemplateReplacements(PageRenderSession session, ICollection<Replacement> replacements)
    {
        if (!TryGetThemeDictionary(session.App?.Config, out IDictionary<string, object> themeDictionary))
        {
            return;
        }

        PageRenderTemplate baseTemplate = null;
        PageRenderTemplate themeTemplate = null;

        session.App?.TemplatesByName.TryGetValue("Theme", out baseTemplate);
        session.App?.TemplatesByName.TryGetValue("Theme-" + session.Request.Theme, out themeTemplate);

        string baseTheme = baseTemplate == null
            ? string.Empty
            : RenderTemplate(baseTemplate, themeDictionary, session, replacements.ToList());

        themeDictionary.TryGetValue(session.Request.Theme ?? string.Empty, out object themeModel);

        if (themeModel == null && !string.IsNullOrWhiteSpace(session.App?.DefaultTheme))
        {
            themeDictionary.TryGetValue(session.App.DefaultTheme, out themeModel);
        }

        string renderedTheme = themeModel == null || themeTemplate == null
            ? string.Empty
            : RenderTemplate(themeTemplate, themeModel, session, replacements.ToList());

        replacements.Add(new Replacement("[theme[template]]", renderedTheme));
        replacements.Add(new Replacement("[theme[base]]", baseTheme));
    }

    private string RenderTemplate(PageRenderTemplate template, object model, PageRenderSession session, IReadOnlyCollection<Replacement> pageReplacements)
    {
        List<Replacement> replacements = pageReplacements.ToList();
        replacements.Add(new Replacement("[model]", JsonConvert.SerializeObject(model)));
        replacements.AddRange(BuildModelReplacements(model));

        return RenderMarkup(template.ResourceKey, template.RawString, session, replacements, allowContentTags: false);
    }

    private IEnumerable<Replacement> BuildModelReplacements(object model, string prefix = "")
    {
        if (model == null)
        {
            return Array.Empty<Replacement>();
        }

        if (model is string text)
        {
            return [new Replacement("[model[" + prefix + "]]", text)];
        }

        if (model is JObject jObject)
        {
            return BuildJObjectReplacements(jObject, prefix);
        }

        if (model is JArray jArray)
        {
            return BuildCollectionReplacements(jArray, prefix);
        }

        if (model.GetType().GetInterface("IDynamicMetaObjectProvider") != null)
        {
            return BuildDynamicReplacements((IDictionary<string, object>)model, prefix);
        }

        if (model is IEnumerable enumerable && model is not string)
        {
            return BuildCollectionReplacements(enumerable, prefix);
        }

        return BuildObjectReplacements(model, prefix);
    }

    private IEnumerable<Replacement> BuildCollectionReplacements(IEnumerable model, string prefix)
    {
        List<Replacement> replacements = [];
        int index = 0;

        foreach (object item in model)
        {
            replacements.AddRange(BuildModelReplacements(item, $"{prefix}[{index}]"));
            index++;
        }

        string lengthBinding = string.IsNullOrEmpty(prefix) ? "Length" : prefix + ".Length";
        replacements.Add(new Replacement("[model[" + lengthBinding + "]]", index.ToString()));

        return replacements;
    }

    private IEnumerable<Replacement> BuildObjectReplacements(object model, string prefix)
    {
        return model.GetType()
            .GetProperties()
            .SelectMany(property =>
            {
                object value = property.GetValue(model);
                string bindingExpression = string.IsNullOrEmpty(prefix) ? property.Name : prefix + "." + property.Name;

                if (property.PropertyType.IsValueType || property.PropertyType == typeof(string))
                {
                    return [new Replacement("[model[" + bindingExpression + "]]", value?.ToString() ?? string.Empty)];
                }

                return value != null
                    ? BuildModelReplacements(value, bindingExpression)
                    : Array.Empty<Replacement>();
            });
    }

    private IEnumerable<Replacement> BuildJObjectReplacements(JObject model, string prefix)
    {
        return model.Properties().SelectMany(property =>
        {
            string bindingExpression = string.IsNullOrEmpty(prefix) ? property.Name : prefix + "." + property.Name;

            return property.Value is JValue value
                ? [new Replacement("[model[" + bindingExpression + "]]", value.ToString())]
                : BuildModelReplacements(property.Value, bindingExpression);
        });
    }

    private IEnumerable<Replacement> BuildDynamicReplacements(IDictionary<string, object> model, string prefix)
    {
        return model.Keys.SelectMany(key =>
        {
            string bindingExpression = string.IsNullOrEmpty(prefix) ? key : prefix + "." + key;
            object value = model[key];

            List<Replacement> replacements = [new("[model[" + bindingExpression + "]]", value?.ToString() ?? string.Empty)];

            if (value != null && !value.GetType().IsValueType && value is not string)
            {
                replacements.AddRange(BuildModelReplacements(value, bindingExpression));
            }

            return replacements;
        });
    }

    private IEnumerable<Replacement> BuildThemeReplacements(object model, string prefix = "")
    {
        if (model == null)
        {
            return Array.Empty<Replacement>();
        }

        if (model is JObject jObject)
        {
            return BuildThemeJObjectReplacements(jObject, prefix);
        }

        if (model is string text)
        {
            return [new Replacement("[theme[" + prefix + "]]", text)];
        }

        if (model.GetType().GetInterface("IDynamicMetaObjectProvider") != null && model is IDictionary<string, object> dynamicModel)
        {
            return BuildThemeDynamicReplacements(dynamicModel, prefix);
        }

        if (model is IEnumerable enumerable && model is not string)
        {
            return BuildThemeCollectionReplacements(enumerable, prefix);
        }

        return BuildThemeObjectReplacements(model, prefix);
    }

    private IEnumerable<Replacement> BuildThemeCollectionReplacements(IEnumerable model, string prefix)
    {
        string bindingExpression = prefix ?? string.Empty;
        List<Replacement> replacements = [];
        int index = 0;

        foreach (object item in model)
        {
            replacements.AddRange(BuildThemeReplacements(item, bindingExpression + $"[{index}]"));
            index++;
        }

        string lengthBinding = bindingExpression.Length == 0 ? "Length" : bindingExpression + ".Length";
        replacements.Add(new Replacement("[theme[" + lengthBinding + "]]", index.ToString()));

        return replacements;
    }

    private IEnumerable<Replacement> BuildThemeObjectReplacements(object model, string prefix)
    {
        return model.GetType()
            .GetProperties()
            .SelectMany(property =>
            {
                object value = property.GetValue(model);
                string bindingExpression = string.IsNullOrEmpty(prefix) ? property.Name : prefix + "." + property.Name;

                if (property.PropertyType.IsValueType || property.PropertyType == typeof(string))
                {
                    return
                    [
                        new Replacement("[theme[" + prefix + "]]", model?.ToString() ?? string.Empty),
                        new Replacement("[theme[" + bindingExpression + "]]", value?.ToString() ?? string.Empty)
                    ];
                }

                return value != null
                    ? BuildThemeReplacements(value, bindingExpression)
                    : Array.Empty<Replacement>();
            });
    }

    private IEnumerable<Replacement> BuildThemeJObjectReplacements(JObject model, string prefix)
    {
        return model.Properties().SelectMany(property =>
        {
            string bindingExpression = string.IsNullOrEmpty(prefix) ? property.Name : prefix + "." + property.Name;

            return property.Value is JValue value
                ? [new Replacement("[theme[" + bindingExpression + "]]", value.ToString())]
                : BuildThemeReplacements(property.Value, bindingExpression);
        });
    }

    private IEnumerable<Replacement> BuildThemeDynamicReplacements(IDictionary<string, object> model, string prefix)
    {
        return model.Keys.SelectMany(key =>
        {
            string bindingExpression = string.IsNullOrEmpty(prefix) ? key : prefix + "." + key;
            object value = model[key];

            List<Replacement> replacements = [new("[theme[" + bindingExpression + "]]", value?.ToString() ?? string.Empty)];

            if (value != null && !value.GetType().IsValueType)
            {
                replacements.AddRange(BuildThemeReplacements(value, bindingExpression));
            }

            return replacements;
        });
    }

    private static bool TryGetThemeDictionary(object config, out IDictionary<string, object> themeDictionary)
    {
        themeDictionary = null;

        if (config is not IDictionary<string, object> dictionary)
        {
            return false;
        }

        if (!dictionary.TryGetValue("Themes", out object value))
        {
            return false;
        }

        themeDictionary = value as IDictionary<string, object>;
        return themeDictionary != null;
    }

    private PageRenderResource ResolveResource(PageRenderSession session, string key, string name)
    {
        string culture = ResolveCulture(session).ToLowerInvariant();
        string normalizedKey = key.ToLowerInvariant();
        string normalizedName = name.ToLowerInvariant();

        PageRenderResource resource = FindIndexedResource(session.ResourcesByLookup, normalizedKey, normalizedName, culture);

        if (resource != null)
        {
            return resource;
        }

        if (culture.Contains('-'))
        {
            resource = FindIndexedResource(session.ResourcesByLookup, normalizedKey, normalizedName, culture.Split('-')[0]);

            if (resource != null)
            {
                return resource;
            }
        }

        resource = FindIndexedResource(session.ResourcesByLookup, normalizedKey, normalizedName, string.Empty);
        return resource ?? ResolveCommonResource(session, normalizedKey, normalizedName, culture);
    }

    private PageRenderResource ResolveCommonResource(PageRenderSession session, string key, string name, string culture)
    {
        PageRenderResource resource = FindIndexedResource(session.CommonResourcesByLookup, key, name, culture);

        if (resource != null)
        {
            return resource;
        }

        if (culture.Contains('-'))
        {
            resource = FindIndexedResource(session.CommonResourcesByLookup, key, name, culture.Split('-')[0]);

            if (resource != null)
            {
                return resource;
            }
        }

        return FindIndexedResource(session.CommonResourcesByLookup, key, name, string.Empty);
    }

    private static PageRenderResource FindIndexedResource(IReadOnlyDictionary<string, PageRenderResource> lookup, string key, string name, string culture)
    {
        return lookup.TryGetValue(BuildResourceLookupKey(key, name, culture), out PageRenderResource value)
            ? value
            : null;
    }

    private PageRenderComponent ResolveComponent(PageRenderSession session, string name)
    {
        if (session.ComponentsByName.TryGetValue(name, out PageRenderComponent component))
        {
            return component;
        }

        cCoder.Data.Models.CMS.Component dataComponent = componentReaderBroker.GetComponent(session.Request.AppId, name);

        if (dataComponent != null)
        {
            component = new PageRenderComponent
            {
                Id = dataComponent.Id,
                Name = dataComponent.Name ?? string.Empty,
                ResourceKey = dataComponent.ResourceKey ?? string.Empty,
                Content = dataComponent.Content ?? string.Empty,
                Script = dataComponent.Script ?? string.Empty
            };

            session.ComponentsByName[name] = component;
            return component;
        }

        return session.CommonComponentsByName.TryGetValue(name, out component) ? component : null;
    }

    private PageRenderScript ResolveScript(PageRenderSession session, string name)
    {
        if (session.ScriptsByName.TryGetValue(name, out PageRenderScript script))
        {
            return script;
        }

        cCoder.Data.Models.CMS.Script dataScript = scriptReaderBroker.GetScript(session.Request.AppId, name);

        if (dataScript != null)
        {
            script = new PageRenderScript
            {
                Name = dataScript.Name ?? string.Empty,
                Content = dataScript.Content ?? string.Empty
            };

            session.ScriptsByName[name] = script;
            return script;
        }

        return session.CommonScriptsByName.TryGetValue(name, out script) ? script : null;
    }

    private static string BuildResourceLookupKey(string key, string name, string culture) =>
        $"{key}|{name}|{culture}";

    private static string ResolveCulture(PageRenderSession session) =>
        !string.IsNullOrWhiteSpace(session.Request.Culture)
            ? session.Request.Culture
            : session.App?.DefaultCulture ?? string.Empty;

    private static string GetName(Match match) =>
        match.Groups["name"].Value.ToLowerInvariant();

    private static string[] GetOptions(Match match) =>
        match.Groups["options"].Value.Split("|", StringSplitOptions.RemoveEmptyEntries);

    private static void RegexReplace(StringBuilder source, Regex regex, Func<Match, string> action)
    {
        string result = regex.Replace(source.ToString(), match => action(match));
        source.Clear();
        source.Append(result);
    }
}
