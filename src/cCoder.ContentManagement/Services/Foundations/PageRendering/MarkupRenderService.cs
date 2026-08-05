// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Rendering.Brokers;
using cCoder.ContentManagement.Models.PageRendering;
using cCoder.ContentManagement.Services.Foundations;
using cCoder.ContentManagement.Services.Processings.PageRendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace cCoder.ContentManagement.Rendering.Services.Foundations;

internal sealed partial class MarkupRenderService(
    IRenderBroker renderBroker) : IMarkupRenderService
{
    private static readonly RegexOptions regexOptions = RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline;

    private static readonly Regex elementRegex = new(
        pattern: "<(?<tag>script|style)\\b",
        options: RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex nonceRegex = new(
        pattern: "\\s+nonce\\s*=\\s*(?:'[^']*'|\"[^\"]*\"|[^\\s>]+)",
        options: RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private const string NonceAttribute =
        "nonce='" + ContentSecurityPolicyNonceContract.Placeholder + "'";

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

    public PageRenderSession RenderPageRenderSession(PageRenderSession session) =>
        TryCatch<PageRenderSession>(operation: () =>
    {
        ValidateRenderPageRenderSession(inputs: [session]);
        string key = string.IsNullOrWhiteSpace(value: session.Page?.ResourceKey) ? "Default" : session.Page.ResourceKey;
        string culture = ResolveCulture(session: session);

        List<ReplacementDependency> replacements = BuildDefaultReplacements(session: session)
            .ToList();

        AddThemeTemplateReplacements(newPageRenderSession: session, newReplacement: replacements);

        session.Result = new PageRenderResult
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
            HeaderHtml = MarkContentSecurityPolicyNonce(markup: RenderMarkup(key: key, content: session.Layout?.HeaderHtml ?? string.Empty, session: session, replacements: replacements, allowContentTags: false)),
            BodyHtml = session.Request.HeaderOnly
                ? string.Empty
                : MarkContentSecurityPolicyNonce(markup: RenderMarkup(key: key, content: session.Layout?.BodyHtml ?? string.Empty, session: session, replacements: replacements)),
            StatusCode = session.Page == null ? 404 : 200
        };

        return session;

    });

    private string RenderMarkup(string key, string content, PageRenderSession session, IReadOnlyCollection<ReplacementDependency> replacements, bool allowContentTags = true)
    {
        if (string.IsNullOrEmpty(value: content))
        {
            return string.Empty;
        }

        StringBuilder result = new(value: content, capacity: content.Length * 4);

        TagHandlingOperation tagHandlingOperation = HandleTags(
            operation: new TagHandlingOperation
            {
                Session = session,
                ResourceKey = key,
                Content = result.ToString(),
                AllowContentTags = allowContentTags,
                Replacements = replacements,
                Fragments = new List<TagHandlingFragment>()
            });

        result.Clear();
        result.Append(value: tagHandlingOperation.Content);

        return result.ToString();
    }

    private TagHandlingOperation HandleTags(TagHandlingOperation operation)
    {
        ITagHandlingProcessingService[] handlers =
        [
            .. renderBroker.GetTagHandlers()
        ];

        HashSet<string> observedContent = new(
            comparer: StringComparer.Ordinal);

        for (int pass = 0; pass < 64; pass++)
        {
            string contentBeforePass = operation.Content;

            if (!observedContent.Add(item: contentBeforePass))
            {
                throw new InvalidOperationException(
                    message: "Tag rendering entered a replacement cycle.");
            }

            foreach (ITagHandlingProcessingService handler in handlers)
            {
                operation = handler.HandleTagHandlingOperation(
                    operation: operation);
            }

            foreach (TagHandlingFragment fragment in operation.Fragments)
            {
                TagHandlingOperation renderedFragment = HandleTags(
                    operation: fragment.Operation);

                operation.Content = operation.Content.Replace(
                    oldValue: fragment.Token,
                    newValue: renderedFragment.Content);
            }

            operation.Fragments.Clear();

            if (string.Equals(
                a: contentBeforePass,
                b: operation.Content,
                comparisonType: StringComparison.Ordinal))
            {
                return operation;
            }
        }

        throw new InvalidOperationException(
            message: "Tag rendering exceeded the maximum replacement passes.");
    }

    private IEnumerable<ReplacementDependency> BuildDefaultReplacements(PageRenderSession session)
    {
        string culture = ResolveCulture(session: session);

        string port = session.Config?.SslPort is int sslPort
            ? $":{sslPort}"
            : string.Empty;

        PageRenderUser user = session.User ?? new PageRenderUser();
        bool isGuest = string.IsNullOrWhiteSpace(value: user.Id) || string.Equals(a: user.Id, b: "Guest", comparisonType: StringComparison.OrdinalIgnoreCase);
        bool cacheTemplate = session.Request.CacheTemplate;

        List<ReplacementDependency> replacements =
        [
            new(old: "[[user]]", @new: cacheTemplate
                ? PageRenderRuntimeTokens.User
                : JsonConvert.SerializeObject(value: new
            {
                Id = isGuest ? "Guest" : user.Id,
                DefaultCultureId = string.IsNullOrWhiteSpace(value: user.DefaultCultureId) ? culture : user.DefaultCultureId,
                DisplayName = isGuest ? "Guest" : user.DisplayName,
                Email = user.Email ?? string.Empty
            })),
            new(old: "[[displayname]]", @new: cacheTemplate
                ? PageRenderRuntimeTokens.DisplayName
                : isGuest ? "Guest" : user.DisplayName),
            new(old: "[[loginlink]]", @new: cacheTemplate
                ? PageRenderRuntimeTokens.LoginLink
                : isGuest ? "<a href='/Login'>[resource_displayname[Login]]</a>" : "<a name='logout' href=''>[resource_displayname[Logout]]</a>"),
            new(old: "[[date]]", @new: cacheTemplate
                ? PageRenderRuntimeTokens.Date
                : DateTimeOffset.UtcNow.ToString(format: "dd MMM yyyy")),
            new(old: "[[culture]]", @new: WebUtility.HtmlEncode(value: culture)),
            new(old: "[[lang]]", @new: WebUtility.HtmlEncode(value: culture.Split(separator: '-')
            .FirstOrDefault() ?? string.Empty)),
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
            new(old: "[page[path]]", @new: WebUtility.HtmlEncode(value: session.Page?.Path ?? string.Empty)),
            new(old: "[page[url]]", @new: WebUtility.HtmlEncode(value: string.IsNullOrWhiteSpace(value: session.Page?.Path) ? "https://" + session.App?.Domain + "/" : "https://" + session.App?.Domain + "/" + session.Page?.Path)),
            new(old: "[theme[name]]", @new: WebUtility.HtmlEncode(value: session.Request.Theme ?? string.Empty)),
            new(
                old: "[[editlink]]",
                @new: CanPageRenderUser(
                    user: user,
                    appId: session.App?.Id,
                    operation: "page_update")
                        ? "<a href='?edit=true'>Edit</a>"
                        : string.Empty)
        ];

        replacements.AddRange(collection: BuildConfiguredReplacements(session: session));
        replacements.AddRange(collection: BuildThemeValueReplacements(session: session));

        return replacements;
    }

    private static IEnumerable<ReplacementDependency> BuildConfiguredReplacements(PageRenderSession session)
    {
        if (!string.IsNullOrWhiteSpace(
            value: session.Config?.WorkflowServiceUrl))
        {
            yield return new ReplacementDependency(
                old: "[api[workflow]]",
                @new: session.Config.WorkflowServiceUrl);
        }
    }

    private IEnumerable<ReplacementDependency> BuildThemeValueReplacements(PageRenderSession session)
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
            foreach (ReplacementDependency replacement in BuildThemeReplacements(model: requestedTheme))
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

        foreach (ReplacementDependency replacement in BuildThemeReplacements(model: requestedTheme))
        {
            yield return replacement;
        }
    }

    private void AddThemeTemplateReplacements(PageRenderSession newPageRenderSession, ICollection<ReplacementDependency> newReplacement)
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

        newReplacement.Add(item: new ReplacementDependency(old: "[theme[template]]", @new: renderedTheme));
        newReplacement.Add(item: new ReplacementDependency(old: "[theme[base]]", @new: baseTheme));
    }

    private string RenderTemplate(PageRenderTemplate template, object model, PageRenderSession session, IReadOnlyCollection<ReplacementDependency> pageReplacements)
    {
        List<ReplacementDependency> replacements = pageReplacements.ToList();
        replacements.Add(item: new ReplacementDependency(old: "[model]", @new: JsonConvert.SerializeObject(value: model)));
        replacements.AddRange(collection: BuildModelReplacements(model: model));

        return RenderMarkup(key: template.ResourceKey, content: template.RawString, session: session, replacements: replacements, allowContentTags: false);
    }

    private IEnumerable<ReplacementDependency> BuildModelReplacements(object model, string prefix = "")
    {
        if (model == null)
        {
            return Array.Empty<ReplacementDependency>();
        }

        if (model is string text)
        {
            return [new ReplacementDependency(old: "[model[" + prefix + "]]", @new: text)];
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

    private IEnumerable<ReplacementDependency> BuildCollectionReplacements(IEnumerable model, string prefix)
    {
        List<ReplacementDependency> replacements = [];
        int index = 0;

        foreach (object item in model)
        {
            replacements.AddRange(collection: BuildModelReplacements(model: item, prefix: $"{prefix}[{index}]"));
            index++;
        }

        string lengthBinding = string.IsNullOrEmpty(value: prefix) ? "Length" : prefix + ".Length";
        replacements.Add(item: new ReplacementDependency(old: "[model[" + lengthBinding + "]]", @new: index.ToString()));

        return replacements;
    }

    private IEnumerable<ReplacementDependency> BuildObjectReplacements(object model, string prefix) =>
        model.GetType()
        .GetProperties()
        .SelectMany(selector: property =>
            {
                object value = property.GetValue(obj: model);
                string bindingExpression = string.IsNullOrEmpty(value: prefix) ? property.Name : prefix + "." + property.Name;

                if (property.PropertyType.IsValueType || property.PropertyType == typeof(string))
                {
                    return [new ReplacementDependency(old: "[model[" + bindingExpression + "]]", @new: value?.ToString() ?? string.Empty)];
                }

                return value != null
                    ? BuildModelReplacements(model: value, prefix: bindingExpression)
                    : Array.Empty<ReplacementDependency>();
            });

    private IEnumerable<ReplacementDependency> BuildJObjectReplacements(JObject model, string prefix) =>
        model.Properties()
        .SelectMany(selector: property =>
        {
            string bindingExpression = string.IsNullOrEmpty(value: prefix) ? property.Name : prefix + "." + property.Name;

            return property.Value is JValue value
                ? [new ReplacementDependency(old: "[model[" + bindingExpression + "]]", @new: value.ToString())]
                : BuildModelReplacements(model: property.Value, prefix: bindingExpression);
        });

    private IEnumerable<ReplacementDependency> BuildDynamicReplacements(IDictionary<string, object> model, string prefix) =>
        model.Keys.SelectMany(selector: key =>
                                                                                                                           {
                                                                                                                               string bindingExpression = string.IsNullOrEmpty(value: prefix) ? key : prefix + "." + key;
                                                                                                                               object value = model[key];

                                                                                                                               List<ReplacementDependency> replacements = [new(old: "[model[" + bindingExpression + "]]", @new: value?.ToString() ?? string.Empty)];

                                                                                                                               if (value != null && !value.GetType()
                                                                                                                                   .IsValueType && value is not string)
                                                                                                                               {
                                                                                                                                   replacements.AddRange(collection: BuildModelReplacements(model: value, prefix: bindingExpression));
                                                                                                                               }

                                                                                                                               return replacements;
                                                                                                                           });

    private IEnumerable<ReplacementDependency> BuildThemeReplacements(object model, string prefix = "")
    {
        if (model == null)
        {
            return Array.Empty<ReplacementDependency>();
        }

        if (model is JObject jObject)
        {
            return BuildThemeJObjectReplacements(model: jObject, prefix: prefix);
        }

        if (model is string text)
        {
            return [new ReplacementDependency(old: "[theme[" + prefix + "]]", @new: text)];
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

    private IEnumerable<ReplacementDependency> BuildThemeCollectionReplacements(IEnumerable model, string prefix)
    {
        string bindingExpression = prefix ?? string.Empty;
        List<ReplacementDependency> replacements = [];
        int index = 0;

        foreach (object item in model)
        {
            replacements.AddRange(collection: BuildThemeReplacements(model: item, prefix: bindingExpression + $"[{index}]"));
            index++;
        }

        string lengthBinding = bindingExpression.Length == 0 ? "Length" : bindingExpression + ".Length";
        replacements.Add(item: new ReplacementDependency(old: "[theme[" + lengthBinding + "]]", @new: index.ToString()));

        return replacements;
    }

    private IEnumerable<ReplacementDependency> BuildThemeObjectReplacements(object model, string prefix) =>
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
                        new ReplacementDependency(old: "[theme[" + prefix + "]]", @new: model?.ToString() ?? string.Empty),
                        new ReplacementDependency(old: "[theme[" + bindingExpression + "]]", @new: value?.ToString() ?? string.Empty)
                    ];
                }

                return value != null
                    ? BuildThemeReplacements(model: value, prefix: bindingExpression)
                    : Array.Empty<ReplacementDependency>();
            });

    private IEnumerable<ReplacementDependency> BuildThemeJObjectReplacements(JObject model, string prefix) =>
        model.Properties()
        .SelectMany(selector: property =>
        {
            string bindingExpression = string.IsNullOrEmpty(value: prefix) ? property.Name : prefix + "." + property.Name;

            return property.Value is JValue value
                ? [new ReplacementDependency(old: "[theme[" + bindingExpression + "]]", @new: value.ToString())]
                : BuildThemeReplacements(model: property.Value, prefix: bindingExpression);
        });

    private IEnumerable<ReplacementDependency> BuildThemeDynamicReplacements(IDictionary<string, object> model, string prefix) =>
        model.Keys.SelectMany(selector: key =>
                                                                                                                                {
                                                                                                                                    string bindingExpression = string.IsNullOrEmpty(value: prefix) ? key : prefix + "." + key;
                                                                                                                                    object value = model[key];

                                                                                                                                    List<ReplacementDependency> replacements = [new(old: "[theme[" + bindingExpression + "]]", @new: value?.ToString() ?? string.Empty)];

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

    internal static string MarkContentSecurityPolicyNonce(string markup)
    {
        if (string.IsNullOrEmpty(value: markup))
        {
            return markup ?? string.Empty;
        }

        StringBuilder result = new(capacity: markup.Length + 64);
        int position = 0;

        while (TryFindElement(
            markup: markup,
            startIndex: position,
            tagName: out string tagName,
            openingStart: out int openingStart))
        {
            int openingEnd = FindTagEnd(
                markup: markup,
                startIndex: openingStart + tagName.Length + 1);

            if (openingEnd < 0)
            {
                break;
            }

            result.Append(
                value: markup,
                startIndex: position,
                count: openingStart - position);

            string openingTag = markup.Substring(
                startIndex: openingStart,
                length: openingEnd - openingStart + 1);

            result.Append(value: MarkOpeningTag(openingTag: openingTag));
            int contentStart = openingEnd + 1;

            int closingStart = markup.IndexOf(
                value: "</" + tagName,
                startIndex: contentStart,
                comparisonType: StringComparison.OrdinalIgnoreCase);

            if (closingStart < 0)
            {
                position = contentStart;
                continue;
            }

            result.Append(
                value: markup,
                startIndex: contentStart,
                count: closingStart - contentStart);

            position = closingStart;
        }

        result.Append(
            value: markup,
            startIndex: position,
            count: markup.Length - position);

        return result.ToString();
    }

    private static string MarkOpeningTag(string openingTag)
    {
        string withoutNonce = nonceRegex.Replace(
            input: openingTag,
            replacement: string.Empty);

        int insertAt = withoutNonce.EndsWith(
            value: "/>",
            comparisonType: StringComparison.Ordinal)
                ? withoutNonce.Length - 2
                : withoutNonce.Length - 1;

        return withoutNonce.Insert(
            startIndex: insertAt,
            value: " " + NonceAttribute);
    }

    private static bool TryFindElement(
        string markup,
        int startIndex,
        out string tagName,
        out int openingStart)
    {
        Match match = elementRegex.Match(
            input: markup,
            startat: startIndex);

        tagName = match.Success
            ? match.Groups["tag"].Value
            : string.Empty;

        openingStart = match.Success
            ? match.Index
            : -1;

        return match.Success;
    }

    private static int FindTagEnd(string markup, int startIndex)
    {
        char quote = '\0';

        for (int index = startIndex; index < markup.Length; index++)
        {
            char current = markup[index];

            if (quote == '\0' && (current == '\'' || current == '"'))
            {
                quote = current;
            }
            else if (quote == current)
            {
                quote = '\0';
            }
            else if (quote == '\0' && current == '>')
            {
                return index;
            }
        }

        return -1;
    }
}