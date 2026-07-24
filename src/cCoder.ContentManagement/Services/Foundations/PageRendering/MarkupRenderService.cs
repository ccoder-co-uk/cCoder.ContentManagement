// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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

internal sealed partial class MarkupRenderService(
    IComponentReaderBroker componentReaderBroker,
    IScriptReaderBroker scriptReaderBroker,
    IJsonBroker jsonBroker,
    IRenderFileContentService renderFileContentService) : IMarkupRenderService
{
    private static readonly RegexOptions regexOptions = RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline;

    private static readonly PageRenderSyntax syntax = new()
    {
        ContentRegex = new Regex(pattern: "\\[content\\[(?<name>[A-Za-z\\d_\\-/. ]+)\\](?<options>[^\\]]*)\\]", options: regexOptions),
        ResourceDisplayNameRegex = new Regex(pattern: "\\[resource_displayname\\[(?<name>[A-Za-z\\d_\\-/. ]+)\\]\\]", options: regexOptions),
        ResourceShortDisplayNameRegex = new Regex(pattern: "\\[resource_shortdisplayname\\[(?<name>[A-Za-z\\d_\\-/. ]+)\\]\\]", options: regexOptions),
        ResourceDescriptionRegex = new Regex(pattern: "\\[resource_description\\[(?<name>[A-Za-z\\d_\\-/. ]+)\\]\\]", options: regexOptions),
        NavRegex = new Regex(pattern: "\\[nav\\[(?<name>[A-Za-z\\d_\\-/. ]*)\\]\\]", options: regexOptions),
        NavExpandedRegex = new Regex(pattern: "\\[navExpanded\\[(?<name>[A-Za-z\\d_\\-/. ]*)\\]\\]", options: regexOptions),
        DmsRegex = new Regex(pattern: "\\[dms\\[(?<name>[A-Za-z\\d_\\-/. ]+)\\]\\]", options: regexOptions),
        ExecuteRegex = new Regex(pattern: "\\[execute\\](.*?)\\[/execute\\]", options: regexOptions),
        ComponentRegex = new Regex(pattern: "\\[component\\[(?<name>[A-Za-z\\d_\\-/. ]+)\\](?<options>[^\\]]*)\\]", options: regexOptions),
        ScriptRegex = new Regex(pattern: "\\[script\\[(?<name>[A-Za-z\\d_\\-/. ]+)\\]\\]", options: regexOptions),
        MetaRegex = new Regex(pattern: "\\[meta\\[(?<name>[A-Za-z\\d_\\-/. ]+)\\]\\]", options: regexOptions),
        CultureLinkRegex = new Regex(pattern: "\\[culturelink\\[(?<name>[A-Za-z\\d_\\-/. ]+)\\]\\]", options: regexOptions)
    };

    public PageRenderResult RenderPageRenderSessionPageRenderResult(PageRenderSession session) =>
        TryCatch<PageRenderResult>(operation: () =>
    {
        ValidateRenderPageRenderSessionPageRenderResult(inputs: [session]);
        string key = string.IsNullOrWhiteSpace(value: session.Page?.ResourceKey) ? "Default" : session.Page.ResourceKey;
        string culture = ResolveCulture(session: session);

        List<Replacement> replacements = BuildDefaultReplacements(session: session)
            .ToList();

        AddThemeTemplateReplacements(newPageRenderSession: session, newReplacement: replacements);

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
            HeaderHtml = RenderMarkup(key: key, content: session.Layout?.HeaderHtml ?? string.Empty, session: session, replacements: replacements, allowContentTags: false),
            BodyHtml = RenderMarkup(key: key, content: session.Layout?.BodyHtml ?? string.Empty, session: session, replacements: replacements),
            StatusCode = session.Page == null ? 404 : 200
        };

    });

    private string RenderMarkup(string key, string content, PageRenderSession session, IReadOnlyCollection<Replacement> replacements, bool allowContentTags = true)
    {
        if (string.IsNullOrEmpty(value: content))
        {
            return string.Empty;
        }

        StringBuilder result = new(value: content, capacity: content.Length * 4);

        if (allowContentTags)
        {
            Content(key: key, source: result, session: session, replacements: replacements);
        }

        Nav(source: result, session: session);
        Dms(key: key, source: result, session: session, replacements: replacements);
        Script(key: key, source: result, session: session, replacements: replacements);
        RegexReplace(source: result, regex: syntax.CultureLinkRegex, action: static unusedMatch => "?culture=");
        Component(key: key, session: session, replacements: replacements, result: result);
        Meta(source: result, session: session);
        Resource(source: result, session: session, key: key, replacements: replacements);
        ExecuteAsync(key: key, source: result, session: session, replacements: replacements);

        foreach (Replacement replacement in replacements)
        {
            result.Replace(oldValue: replacement.Old, newValue: replacement.New);
        }

        return result.ToString();
    }

    private void Content(string key, StringBuilder source, PageRenderSession session, IReadOnlyCollection<Replacement> replacements) =>
        RegexReplace(source: source, regex: syntax.ContentRegex, action: match =>
                                                                                                                                        {
                                                                                                                                            string name = GetName(match: match);
                                                                                                                                            string[] options = GetOptions(match: match);
                                                                                                                                            PageRenderContent pageRenderContent = null;

                                                                                                                                            if (session.Page != null && session.Page.ContentByName.TryGetValue(key: name, value: out PageRenderContent value))
                                                                                                                                            {
                                                                                                                                                pageRenderContent = value;
                                                                                                                                            }

                                                                                                                                            string optionalClass = string.Join(separator: " ", values: options.Where(predicate: option => option.StartsWith(value: "class="))
                                                                                                                                                .Select(selector: option => option.Replace(oldValue: "class=", newValue: string.Empty)));

                                                                                                                                            string contentEditable = session.Request.Edit ? "contenteditable" : string.Empty;

                                                                                                                                            if (pageRenderContent == null)
                                                                                                                                            {
                                                                                                                                                return "[[Missing Content:" + name + "]]";
                                                                                                                                            }

                                                                                                                                            string html = $"<section name='{name}' class='content {optionalClass}' data-id='{pageRenderContent.Id}' {contentEditable} {string.Join(separator: " ", values: options.Where(predicate: option => !option.StartsWith(value: "class=")))}>\n                        {(session.Request.Edit ? pageRenderContent.Html : RenderMarkup(key: key, content: pageRenderContent.Html, session: session, replacements: replacements))}\n                    </section>";

                                                                                                                                            return session.Request.Edit
                                                                                                                                                ? html
                                                                                                                                                : RenderMarkup(key: key, content: html, session: session, replacements: replacements, allowContentTags: false);
                                                                                                                                        });

    private void Nav(StringBuilder source, PageRenderSession session)
    {
        RegexReplace(source: source, regex: syntax.NavRegex, action: match => BuildMenuFor(tagName: GetName(match: match), expand: false));
        RegexReplace(source: source, regex: syntax.NavExpandedRegex, action: match => BuildMenuFor(tagName: GetName(match: match), expand: true));

        string BuildMenuFor(string tagName, bool expand)
        {
            PageRenderPage page = null;

            if (int.TryParse(s: tagName, result: out int pageId) && session.App != null)
            {
                session.App.PagesById.TryGetValue(key: pageId, value: out page);
            }

            return "<div class='collapse navbar-collapse'><ul class='navbar-nav'>" + BuildMenuItemsFor(page: page, expand: expand) + "</ul></div>";
        }

        string BuildMenuItemsFor(PageRenderPage page, bool expand)
        {
            if (session.App == null)
            {
                return string.Empty;
            }

            return string.Join(
separator: "",
values: session.App.PagesById.Values
                    .Where(predicate: subPage => subPage.ParentId == page?.Id && subPage.ShowOnMenus)
                .OrderBy(keySelector: subPage => subPage.Order)
                .Select(selector: subPage =>
                    {
                        string selected = subPage.ParentId.HasValue
                            && page != null
                            && !string.IsNullOrWhiteSpace(value: session.Page?.Path)
                            && session.Page.Path.Contains(value: subPage.Path)
                                ? " active"
                                : string.Empty;

                        return expand
                            ? $"<li data-id='{subPage.Id}' class='nav-item'><a href='/{subPage.Path}' class='nav-link{selected}'>{subPage.Title}</a><ul class='submenu dropdown-menu'>{BuildMenuItemsFor(page: subPage, expand: true)}</ul></li>"
                            : $"<li data-id='{subPage.Id}' class='nav-item'><a href='/{subPage.Path}' class='nav-link{selected}'>{subPage.Title}</a></li>";
                    }));
        }
    }

    private void Dms(string key, StringBuilder source, PageRenderSession session, IReadOnlyCollection<Replacement> replacements) =>
        RegexReplace(source: source, regex: syntax.DmsRegex, action: match =>
                                                                                                                                    {
                                                                                                                                        string name = GetName(match: match);
                                                                                                                                        string latestTextContent = renderFileContentService.GetLatestTextContent(appId: session.App?.Id ?? 0, path: name);

                                                                                                                                        return string.IsNullOrEmpty(value: latestTextContent)
                                                                                                                                            ? string.Empty
                                                                                                                                            : RenderMarkup(key: key, content: latestTextContent, session: session, replacements: replacements, allowContentTags: false);
                                                                                                                                    });

    private void Component(string key, PageRenderSession session, IReadOnlyCollection<Replacement> replacements, StringBuilder result)
    {
        if (session.Request.Edit)
        {
            return;
        }

        RegexReplace(source: result, regex: syntax.ComponentRegex, action: match =>
        {
            string name = GetName(match: match);
            string[] options = GetOptions(match: match);
            PageRenderComponent component = ResolveComponent(session: session, name: name);

            if (component == null)
            {
                return "[[Missing Component:" + name + "]]";
            }

            string optionalClass = string.Join(separator: " ", values: options.Where(predicate: option => option.StartsWith(value: "class="))
                .Select(selector: option => option.Replace(oldValue: "class=", newValue: string.Empty)));

            string content = $"<section name='{component.Name}' class='component {optionalClass}' data-id='{component.Id}' data-resource-key='{component.ResourceKey}' {string.Join(separator: " ", values: options.Where(predicate: option => !option.StartsWith(value: "class=")))}>\n                        {RenderMarkup(key: component.ResourceKey, content: component.Content, session: session, replacements: replacements, allowContentTags: false)}\n                        <script type='text/javascript'>{RenderMarkup(key: component.ResourceKey, content: component.Script, session: session, replacements: replacements, allowContentTags: false)}</script>\n                    </section>";

            return RenderMarkup(key: component.ResourceKey, content: content, session: session, replacements: replacements, allowContentTags: false);
        });
    }

    private void Script(string key, StringBuilder source, PageRenderSession session, IReadOnlyCollection<Replacement> replacements) =>
        RegexReplace(source: source, regex: syntax.ScriptRegex, action: match =>
                                                                                                                                       {
                                                                                                                                           string name = GetName(match: match);
                                                                                                                                           PageRenderScript script = ResolveScript(session: session, name: name);

                                                                                                                                           return script == null
                                                                                                                                               ? string.Empty
                                                                                                                                               : RenderMarkup(key: key, content: script.Content, session: session, replacements: replacements, allowContentTags: false);
                                                                                                                                       });

    private void ExecuteAsync(string key, StringBuilder source, PageRenderSession session, IReadOnlyCollection<Replacement> replacements) =>
        RegexReplace(source: source, regex: syntax.ExecuteRegex, action: match =>
                                                                                                                                             {
                                                                                                                                                 string code = match.Groups[1].Value;
                                                                                                                                                 string json = replacements.FirstOrDefault(predicate: replacement => replacement.Old == "[model]")?.New ?? "{}";

                                                                                                                                                 using HttpClient httpClient = new(handler: new HttpClientHandler
                                                                                                                                                 {
                                                                                                                                                     AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                                                                                                                                                 })
                                                                                                                                                 {
                                                                                                                                                     BaseAddress = new Uri(uriString: replacements.First(predicate: replacement => replacement.Old == "[api[workflow]]")
                                                                                                                                                     .New),
                                                                                                                                                     Timeout = TimeSpan.FromMinutes(minutes: 10)
                                                                                                                                                 };

                                                                                                                                                 string content = SerializeForOData(model: new
                                                                                                                                                 {
                                                                                                                                                     Script = code,
                                                                                                                                                     Model = jsonBroker.ParseJson(json: json)
                                                                                                                                                 });

                                                                                                                                                 Task<string> task = httpClient
                                                                                                                                                     .PostAsync(requestUri: "ExecuteScript?useDetails=true", content: new StringContent(content: content, encoding: Encoding.UTF8, mediaType: "text/plain"))
                                                                                                                                                     .ContinueWith(continuationFunction: responseTask => responseTask.Result.Content.ReadAsStringAsync())
                                                                                                                                                     .Unwrap();

                                                                                                                                                 task.Wait();

                                                                                                                                                 return RenderMarkup(key: key, content: task.Result, session: session, replacements: replacements, allowContentTags: false);
                                                                                                                                             });

    private static string SerializeForOData(object model) =>
        JsonConvert.SerializeObject(value: model, formatting: Formatting.None, settings: new JsonSerializerSettings
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

    private void Meta(StringBuilder source, PageRenderSession session) =>
        RegexReplace(source: source, regex: syntax.MetaRegex, action: match => session.MetadataResolver(arg: GetName(match: match)) ?? string.Empty);

    private void Resource(StringBuilder source, PageRenderSession session, string key, IReadOnlyCollection<Replacement> replacements)
    {
        if (session.Request.Edit)
        {
            return;
        }

        RegexReplace(source: source, regex: syntax.ResourceDisplayNameRegex, action: match =>
            RenderMarkup(key: key, content: ResolveResource(session: session, key: key, name: GetName(match: match))?.DisplayName ?? GetName(match: match), session: session, replacements: replacements, allowContentTags: false));

        RegexReplace(source: source, regex: syntax.ResourceShortDisplayNameRegex, action: match =>
            RenderMarkup(key: key, content: ResolveResource(session: session, key: key, name: GetName(match: match))?.ShortDisplayName ?? GetName(match: match), session: session, replacements: replacements, allowContentTags: false));

        RegexReplace(source: source, regex: syntax.ResourceDescriptionRegex, action: match =>
            RenderMarkup(key: key, content: ResolveResource(session: session, key: key, name: GetName(match: match))?.Description ?? GetName(match: match), session: session, replacements: replacements, allowContentTags: false));
    }

    private IEnumerable<Replacement> BuildDefaultReplacements(PageRenderSession session)
    {
        string culture = ResolveCulture(session: session);
        string port = session.Config != null && session.Config.Settings.TryGetValue(key: "sslPort", value: out string value) ? ":" + value : string.Empty;
        PageRenderUser user = session.User ?? new PageRenderUser();
        bool isGuest = string.IsNullOrWhiteSpace(value: user.Id) || string.Equals(a: user.Id, b: "Guest", comparisonType: StringComparison.OrdinalIgnoreCase);

        List<Replacement> replacements =
        [
            new(old: "[[user]]", @new: JsonConvert.SerializeObject(value: new
            {
                Id = isGuest ? "Guest" : user.Id,
                DefaultCultureId = string.IsNullOrWhiteSpace(value: user.DefaultCultureId) ? culture : user.DefaultCultureId,
                DisplayName = isGuest ? "Guest" : user.DisplayName,
                Email = user.Email ?? string.Empty
            })),
            new(old: "[[displayname]]", @new: isGuest ? "Guest" : user.DisplayName),
            new(old: "[[loginlink]]", @new: isGuest ? "<a href='/Login'>[resource_displayname[Login]]</a>" : "<a name='logout' href=''>[resource_displayname[Logout]]</a>"),
            new(old: "[[date]]", @new: DateTimeOffset.UtcNow.ToString(format: "dd MMM yyyy")),
            new(old: "[[culture]]", @new: culture),
            new(old: "[[lang]]", @new: culture.Split(separator: '-')
            .FirstOrDefault() ?? string.Empty),
            new(old: "[app[name]]", @new: session.App?.Name ?? string.Empty),
            new(old: "[app[domain]]", @new: session.App?.Domain ?? string.Empty),
            new(old: "[app[root]]", @new: "https://" + session.App?.Domain + port + "/"),
            new(old: "[app[id]]", @new: session.App?.Id.ToString() ?? string.Empty),
            new(old: "[api[root]]", @new: "https://" + session.App?.Domain + port + "/Api/"),
            new(old: "[page[title]]", @new: session.Page?.Title ?? string.Empty),
            new(old: "[page[description]]", @new: session.Page?.Description ?? string.Empty),
            new(old: "[page[keywords]]", @new: session.Page?.Keywords ?? string.Empty),
            new(old: "[page[id]]", @new: session.Page?.Id.ToString() ?? string.Empty),
            new(old: "[page[parentid]]", @new: session.Page?.ParentId?.ToString() ?? string.Empty),
            new(old: "[page[path]]", @new: session.Page?.Path ?? string.Empty),
            new(old: "[page[url]]", @new: string.IsNullOrWhiteSpace(value: session.Page?.Path) ? "https://" + session.App?.Domain + "/" : "https://" + session.App?.Domain + "/" + session.Page?.Path),
            new(old: "[theme[name]]", @new: session.Request.Theme ?? string.Empty),
            new(
                old: "[[editlink]]",
                @new: CanPageRenderUser(
                    user: user,
                    appId: session.App?.Id,
                    operation: "page_update")
                        ? "<p style='cursor:pointer' onclick=\"setQueryParameter('edit', true)\">Edit</p>"
                        : string.Empty)
        ];

        replacements.AddRange(collection: BuildConfiguredReplacements(session: session));
        replacements.AddRange(collection: BuildThemeValueReplacements(session: session));

        return replacements;
    }

    private static IEnumerable<Replacement> BuildConfiguredReplacements(PageRenderSession session)
    {
        if (session.Config != null
            && session.Config.Services.TryGetValue(key: "Workflow", value: out string workflowService)
            && !string.IsNullOrWhiteSpace(value: workflowService))
        {
            yield return new Replacement(old: "[api[workflow]]", @new: workflowService);
        }
    }

    private IEnumerable<Replacement> BuildThemeValueReplacements(PageRenderSession session)
    {
        if (!TryGetThemeDictionary(config: session.App?.Config, themeDictionary: out IDictionary<string, object> themeDictionary))
        {
            yield break;
        }

        object requestedTheme = null;

        if (!string.IsNullOrWhiteSpace(value: session.Request.Theme)
            && themeDictionary.TryGetValue(key: session.Request.Theme, value: out requestedTheme)
            && requestedTheme != null)
        {
            foreach (Replacement replacement in BuildThemeReplacements(model: requestedTheme))
            {
                yield return replacement;
            }

            yield break;
        }

        if (string.IsNullOrWhiteSpace(value: session.App?.DefaultTheme)
            || !themeDictionary.TryGetValue(key: session.App.DefaultTheme, value: out requestedTheme)
            || requestedTheme == null)
        {
            yield break;
        }

        foreach (Replacement replacement in BuildThemeReplacements(model: requestedTheme))
        {
            yield return replacement;
        }
    }

    private void AddThemeTemplateReplacements(PageRenderSession newPageRenderSession, ICollection<Replacement> newReplacement)
    {
        if (!TryGetThemeDictionary(config: newPageRenderSession.App?.Config, themeDictionary: out IDictionary<string, object> themeDictionary))
        {
            return;
        }

        PageRenderTemplate baseTemplate = null;
        PageRenderTemplate themeTemplate = null;

        newPageRenderSession.App?.TemplatesByName.TryGetValue(key: "Theme", value: out baseTemplate);
        newPageRenderSession.App?.TemplatesByName.TryGetValue(key: "Theme-" + newPageRenderSession.Request.Theme, value: out themeTemplate);

        string baseTheme = baseTemplate == null
            ? string.Empty
            : RenderTemplate(template: baseTemplate, model: themeDictionary, session: newPageRenderSession, pageReplacements: newReplacement.ToList());

        themeDictionary.TryGetValue(key: newPageRenderSession.Request.Theme ?? string.Empty, value: out object themeModel);

        if (themeModel == null && !string.IsNullOrWhiteSpace(value: newPageRenderSession.App?.DefaultTheme))
        {
            themeDictionary.TryGetValue(key: newPageRenderSession.App.DefaultTheme, value: out themeModel);
        }

        string renderedTheme = themeModel == null || themeTemplate == null
            ? string.Empty
            : RenderTemplate(template: themeTemplate, model: themeModel, session: newPageRenderSession, pageReplacements: newReplacement.ToList());

        newReplacement.Add(item: new Replacement(old: "[theme[template]]", @new: renderedTheme));
        newReplacement.Add(item: new Replacement(old: "[theme[base]]", @new: baseTheme));
    }

    private string RenderTemplate(PageRenderTemplate template, object model, PageRenderSession session, IReadOnlyCollection<Replacement> pageReplacements)
    {
        List<Replacement> replacements = pageReplacements.ToList();
        replacements.Add(item: new Replacement(old: "[model]", @new: JsonConvert.SerializeObject(value: model)));
        replacements.AddRange(collection: BuildModelReplacements(model: model));

        return RenderMarkup(key: template.ResourceKey, content: template.RawString, session: session, replacements: replacements, allowContentTags: false);
    }

    private IEnumerable<Replacement> BuildModelReplacements(object model, string prefix = "")
    {
        if (model == null)
        {
            return Array.Empty<Replacement>();
        }

        if (model is string text)
        {
            return [new Replacement(old: "[model[" + prefix + "]]", @new: text)];
        }

        if (model is JObject jObject)
        {
            return BuildJObjectReplacements(model: jObject, prefix: prefix);
        }

        if (model is JArray jArray)
        {
            return BuildCollectionReplacements(model: jArray, prefix: prefix);
        }

        if (model.GetType()
            .GetInterface(name: "IDynamicMetaObjectProvider") != null)
        {
            return BuildDynamicReplacements(model: (IDictionary<string, object>)model, prefix: prefix);
        }

        if (model is IEnumerable enumerable && model is not string)
        {
            return BuildCollectionReplacements(model: enumerable, prefix: prefix);
        }

        return BuildObjectReplacements(model: model, prefix: prefix);
    }

    private IEnumerable<Replacement> BuildCollectionReplacements(IEnumerable model, string prefix)
    {
        List<Replacement> replacements = [];
        int index = 0;

        foreach (object item in model)
        {
            replacements.AddRange(collection: BuildModelReplacements(model: item, prefix: $"{prefix}[{index}]"));
            index++;
        }

        string lengthBinding = string.IsNullOrEmpty(value: prefix) ? "Length" : prefix + ".Length";
        replacements.Add(item: new Replacement(old: "[model[" + lengthBinding + "]]", @new: index.ToString()));

        return replacements;
    }

    private IEnumerable<Replacement> BuildObjectReplacements(object model, string prefix) =>
        model.GetType()
        .GetProperties()
        .SelectMany(selector: property =>
            {
                object value = property.GetValue(obj: model);
                string bindingExpression = string.IsNullOrEmpty(value: prefix) ? property.Name : prefix + "." + property.Name;

                if (property.PropertyType.IsValueType || property.PropertyType == typeof(string))
                {
                    return [new Replacement(old: "[model[" + bindingExpression + "]]", @new: value?.ToString() ?? string.Empty)];
                }

                return value != null
                    ? BuildModelReplacements(model: value, prefix: bindingExpression)
                    : Array.Empty<Replacement>();
            });

    private IEnumerable<Replacement> BuildJObjectReplacements(JObject model, string prefix) =>
        model.Properties()
        .SelectMany(selector: property =>
        {
            string bindingExpression = string.IsNullOrEmpty(value: prefix) ? property.Name : prefix + "." + property.Name;

            return property.Value is JValue value
                ? [new Replacement(old: "[model[" + bindingExpression + "]]", @new: value.ToString())]
                : BuildModelReplacements(model: property.Value, prefix: bindingExpression);
        });

    private IEnumerable<Replacement> BuildDynamicReplacements(IDictionary<string, object> model, string prefix) =>
        model.Keys.SelectMany(selector: key =>
                                                                                                                           {
                                                                                                                               string bindingExpression = string.IsNullOrEmpty(value: prefix) ? key : prefix + "." + key;
                                                                                                                               object value = model[key];

                                                                                                                               List<Replacement> replacements = [new(old: "[model[" + bindingExpression + "]]", @new: value?.ToString() ?? string.Empty)];

                                                                                                                               if (value != null && !value.GetType()
                                                                                                                                   .IsValueType && value is not string)
                                                                                                                               {
                                                                                                                                   replacements.AddRange(collection: BuildModelReplacements(model: value, prefix: bindingExpression));
                                                                                                                               }

                                                                                                                               return replacements;
                                                                                                                           });

    private IEnumerable<Replacement> BuildThemeReplacements(object model, string prefix = "")
    {
        if (model == null)
        {
            return Array.Empty<Replacement>();
        }

        if (model is JObject jObject)
        {
            return BuildThemeJObjectReplacements(model: jObject, prefix: prefix);
        }

        if (model is string text)
        {
            return [new Replacement(old: "[theme[" + prefix + "]]", @new: text)];
        }

        if (model.GetType()
            .GetInterface(name: "IDynamicMetaObjectProvider") != null && model is IDictionary<string, object> dynamicModel)
        {
            return BuildThemeDynamicReplacements(model: dynamicModel, prefix: prefix);
        }

        if (model is IEnumerable enumerable && model is not string)
        {
            return BuildThemeCollectionReplacements(model: enumerable, prefix: prefix);
        }

        return BuildThemeObjectReplacements(model: model, prefix: prefix);
    }

    private IEnumerable<Replacement> BuildThemeCollectionReplacements(IEnumerable model, string prefix)
    {
        string bindingExpression = prefix ?? string.Empty;
        List<Replacement> replacements = [];
        int index = 0;

        foreach (object item in model)
        {
            replacements.AddRange(collection: BuildThemeReplacements(model: item, prefix: bindingExpression + $"[{index}]"));
            index++;
        }

        string lengthBinding = bindingExpression.Length == 0 ? "Length" : bindingExpression + ".Length";
        replacements.Add(item: new Replacement(old: "[theme[" + lengthBinding + "]]", @new: index.ToString()));

        return replacements;
    }

    private IEnumerable<Replacement> BuildThemeObjectReplacements(object model, string prefix) =>
        model.GetType()
        .GetProperties()
        .SelectMany(selector: property =>
            {
                object value = property.GetValue(obj: model);
                string bindingExpression = string.IsNullOrEmpty(value: prefix) ? property.Name : prefix + "." + property.Name;

                if (property.PropertyType.IsValueType || property.PropertyType == typeof(string))
                {
                    return
                    [
                        new Replacement(old: "[theme[" + prefix + "]]", @new: model?.ToString() ?? string.Empty),
                        new Replacement(old: "[theme[" + bindingExpression + "]]", @new: value?.ToString() ?? string.Empty)
                    ];
                }

                return value != null
                    ? BuildThemeReplacements(model: value, prefix: bindingExpression)
                    : Array.Empty<Replacement>();
            });

    private IEnumerable<Replacement> BuildThemeJObjectReplacements(JObject model, string prefix) =>
        model.Properties()
        .SelectMany(selector: property =>
        {
            string bindingExpression = string.IsNullOrEmpty(value: prefix) ? property.Name : prefix + "." + property.Name;

            return property.Value is JValue value
                ? [new Replacement(old: "[theme[" + bindingExpression + "]]", @new: value.ToString())]
                : BuildThemeReplacements(model: property.Value, prefix: bindingExpression);
        });

    private IEnumerable<Replacement> BuildThemeDynamicReplacements(IDictionary<string, object> model, string prefix) =>
        model.Keys.SelectMany(selector: key =>
                                                                                                                                {
                                                                                                                                    string bindingExpression = string.IsNullOrEmpty(value: prefix) ? key : prefix + "." + key;
                                                                                                                                    object value = model[key];

                                                                                                                                    List<Replacement> replacements = [new(old: "[theme[" + bindingExpression + "]]", @new: value?.ToString() ?? string.Empty)];

                                                                                                                                    if (value != null && !value.GetType()
                                                                                                                                        .IsValueType)
                                                                                                                                    {
                                                                                                                                        replacements.AddRange(collection: BuildThemeReplacements(model: value, prefix: bindingExpression));
                                                                                                                                    }

                                                                                                                                    return replacements;
                                                                                                                                });

    private static bool TryGetThemeDictionary(object config, out IDictionary<string, object> themeDictionary)
    {
        themeDictionary = null;

        if (config is not IDictionary<string, object> dictionary)
        {
            return false;
        }

        if (!dictionary.TryGetValue(key: "Themes", value: out object value))
        {
            return false;
        }

        themeDictionary = value as IDictionary<string, object>;
        return themeDictionary != null;
    }

    private PageRenderResource ResolveResource(PageRenderSession session, string key, string name)
    {
        string culture = ResolveCulture(session: session)
            .ToLowerInvariant();

        string normalizedKey = key.ToLowerInvariant();
        string normalizedName = name.ToLowerInvariant();

        PageRenderResource resource = FindIndexedResource(lookup: session.ResourcesByLookup, key: normalizedKey, name: normalizedName, culture: culture);

        if (resource != null)
        {
            return resource;
        }

        if (culture.Contains(value: '-'))
        {
            resource = FindIndexedResource(lookup: session.ResourcesByLookup, key: normalizedKey, name: normalizedName, culture: culture.Split(separator: '-')[0]);

            if (resource != null)
            {
                return resource;
            }
        }

        resource = FindIndexedResource(lookup: session.ResourcesByLookup, key: normalizedKey, name: normalizedName, culture: string.Empty);
        return resource ?? ResolveCommonResource(session: session, key: normalizedKey, name: normalizedName, culture: culture);
    }

    private PageRenderResource ResolveCommonResource(PageRenderSession session, string key, string name, string culture)
    {
        PageRenderResource resource = FindIndexedResource(lookup: session.CommonResourcesByLookup, key: key, name: name, culture: culture);

        if (resource != null)
        {
            return resource;
        }

        if (culture.Contains(value: '-'))
        {
            resource = FindIndexedResource(lookup: session.CommonResourcesByLookup, key: key, name: name, culture: culture.Split(separator: '-')[0]);

            if (resource != null)
            {
                return resource;
            }
        }

        return FindIndexedResource(lookup: session.CommonResourcesByLookup, key: key, name: name, culture: string.Empty);
    }

    private static PageRenderResource FindIndexedResource(IReadOnlyDictionary<string, PageRenderResource> lookup, string key, string name, string culture) =>
        lookup.TryGetValue(key: BuildResourceLookupKey(key: key, name: name, culture: culture), value: out PageRenderResource value)
            ? value
            : null;

    private PageRenderComponent ResolveComponent(PageRenderSession session, string name)
    {
        if (session.ComponentsByName.TryGetValue(key: name, value: out PageRenderComponent component))
        {
            return component;
        }

        cCoder.Data.Models.CMS.Component dataComponent = componentReaderBroker.GetComponent(appId: session.Request.AppId, name: name);

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

        return session.CommonComponentsByName.TryGetValue(key: name, value: out component) ? component : null;
    }

    private PageRenderScript ResolveScript(PageRenderSession session, string name)
    {
        if (session.ScriptsByName.TryGetValue(key: name, value: out PageRenderScript script))
        {
            return script;
        }

        cCoder.Data.Models.CMS.Script dataScript = scriptReaderBroker.GetScript(appId: session.Request.AppId, name: name);

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

        return session.CommonScriptsByName.TryGetValue(key: name, value: out script) ? script : null;
    }

    private static string BuildResourceLookupKey(string key, string name, string culture) =>
        $"{key}|{name}|{culture}";

    private static bool CanPageRenderUser(
        PageRenderUser user,
        int? appId,
        string operation)
    {
        string normalizedOperation =
            operation?.ToLowerInvariant() ?? string.Empty;

        if (!appId.HasValue)
        {
            return user.AppPrivileges.Values.Any(
                predicate: privileges =>
                    privileges.Contains(item: normalizedOperation));
        }

        return user.AppPrivileges.TryGetValue(
            key: appId.Value,
            value: out ISet<string> value)
            && value.Contains(item: normalizedOperation);
    }

    private static string ResolveCulture(PageRenderSession session) =>
        !string.IsNullOrWhiteSpace(value: session.Request.Culture)
            ? session.Request.Culture
            : session.App?.DefaultCulture ?? string.Empty;

    private static string GetName(Match match) =>
        match.Groups["name"].Value.ToLowerInvariant();

    private static string[] GetOptions(Match match) =>
        match.Groups["options"].Value.Split(separator: "|", options: StringSplitOptions.RemoveEmptyEntries);

    private static void RegexReplace(StringBuilder source, Regex regex, Func<Match, string> action)
    {
        string result = regex.Replace(input: source.ToString(), evaluator: match => action(arg: match));
        source.Clear();
        source.Append(value: result);
    }
}