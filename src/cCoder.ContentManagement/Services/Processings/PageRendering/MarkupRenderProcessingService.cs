// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections;
using System.Net;
using System.Reflection;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Models.PageRendering;
using cCoder.ContentManagement.Services.Foundations;
using cCoder.ContentManagement.Rendering.Services.Foundations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace cCoder.ContentManagement.Rendering.Services.Processings;

internal sealed partial class MarkupRenderProcessingService(
    IMarkupRenderService markupRenderService) : IMarkupRenderProcessingService
{
    public RenderSession RenderRenderSession(RenderSession session) =>
        TryCatch<RenderSession>(operation: () =>
    {
        ValidateRenderRenderSession(inputs: [session]);
        ValidateRenderSession(session: session);

        string key = string.IsNullOrWhiteSpace(value: session.Target?.ResourceKey)
            ? "Default"
            : session.Target.ResourceKey;

        List<ReplacementDependency> replacements = BuildDefaultReplacements(session: session)
            .ToList();

        AddThemeTemplateReplacements(newRenderSession: session, newReplacement: replacements);

        session.Output = new RenderOutput
        {
            HeaderMarkup = MarkupRenderService.MarkContentSecurityPolicyNonce(
                markup: markupRenderService.RenderRenderSessionReplacementDependencies(
                    key: key,
                    content: session.Target?.HeaderMarkup ?? string.Empty,
                    session: session,
                    replacements: replacements,
                    allowContentTags: session.Target?.AllowHeaderContentTags ?? false)),
            BodyMarkup = session.Request.HeaderOnly
                ? string.Empty
                : MarkupRenderService.MarkContentSecurityPolicyNonce(
                    markup: markupRenderService.RenderRenderSessionReplacementDependencies(
                        key: key,
                        content: session.Target?.BodyMarkup ?? string.Empty,
                        session: session,
                        replacements: replacements,
                        allowContentTags: session.Target?.AllowBodyContentTags ?? true))
        };

        return session;

    });

    private IEnumerable<ReplacementDependency> BuildDefaultReplacements(RenderSession session)
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
            .FirstOrDefault() ?? string.Empty))
        ];

        if (session.App != null)
        {
            replacements.AddRange(collection:
            [
                new(old: "[app[name]]", @new: session.App.Name ?? string.Empty),
                new(old: "[app[domain]]", @new: session.App.Domain ?? string.Empty),
                new(old: "[app[root]]", @new: "https://" + session.App.Domain + port + "/"),
                new(old: "[app[id]]", @new: session.App.Id.ToString()),
                new(old: "[api[root]]", @new: "https://" + session.App.Domain + port + "/Api/")
            ]);
        }

        if (session.Page != null)
        {
            string pageRoot = session.App == null
                ? string.Empty
                : "https://" + session.App.Domain + port + "/";

            replacements.AddRange(collection:
            [
                new(old: "[page[title]]", @new: session.Page.Title ?? string.Empty),
                new(old: "[page[description]]", @new: session.Page.Description ?? string.Empty),
                new(old: "[page[keywords]]", @new: session.Page.Keywords ?? string.Empty),
                new(old: "[page[id]]", @new: session.Page.Id.ToString()),
                new(old: "[page[parentid]]", @new: session.Page.ParentId?.ToString() ?? string.Empty),
                new(old: "[page[path]]", @new: WebUtility.HtmlEncode(value: session.Page.Path ?? string.Empty)),
                new(old: "[page[url]]", @new: WebUtility.HtmlEncode(value: pageRoot + (session.Page.Path ?? string.Empty))),
                new(
                    old: "[[editlink]]",
                    @new: CanPageRenderUser(
                        user: user,
                        appId: session.App?.Id,
                        operation: "page_update")
                            ? "<a href='?edit=true'>Edit</a>"
                            : string.Empty)
            ]);
        }

        if (!string.IsNullOrWhiteSpace(value: session.Request.Theme))
        {
            replacements.Add(item: new ReplacementDependency(
                old: "[theme[name]]",
                @new: WebUtility.HtmlEncode(value: session.Request.Theme)));
        }

        if (session.Target?.Model != null)
        {
            replacements.Add(item: new ReplacementDependency(
                old: "[model]",
                @new: JsonConvert.SerializeObject(value: session.Target.Model)));

            replacements.AddRange(collection: BuildModelReplacements(
                model: session.Target.Model));
        }

        replacements.AddRange(collection: BuildConfiguredReplacements(session: session));
        replacements.AddRange(collection: BuildThemeValueReplacements(session: session));

        return replacements;
    }

    private static IEnumerable<ReplacementDependency> BuildConfiguredReplacements(RenderSession session)
    {
        if (!string.IsNullOrWhiteSpace(
            value: session.Config?.WorkflowServiceUrl))
        {
            yield return new ReplacementDependency(
                old: "[api[workflow]]",
                @new: session.Config.WorkflowServiceUrl);
        }
    }

    private IEnumerable<ReplacementDependency> BuildThemeValueReplacements(RenderSession session)
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

    private void AddThemeTemplateReplacements(RenderSession newRenderSession, ICollection<ReplacementDependency> newReplacement)
    {
        if (!TryGetThemeDictionary(config: newRenderSession.App?.Config, themeDictionary: out IDictionary<string, object> themeDictionary))
        {
            return;
        }

        PageRenderTemplate baseTemplate = null;
        PageRenderTemplate themeTemplate = null;

        newRenderSession.App?.TemplatesByName.TryGetValue(key: "Theme", value: out baseTemplate);
        newRenderSession.App?.TemplatesByName.TryGetValue(key: "Theme-" + newRenderSession.Request.Theme, value: out themeTemplate);

        string baseTheme = baseTemplate == null
            ? string.Empty
            : RenderTemplate(template: baseTemplate, model: themeDictionary, session: newRenderSession, pageReplacements: newReplacement.ToList());

        themeDictionary.TryGetValue(key: newRenderSession.Request.Theme ?? string.Empty, value: out object themeModel);

        if (themeModel == null && !string.IsNullOrWhiteSpace(value: newRenderSession.App?.DefaultTheme))
        {
            themeDictionary.TryGetValue(key: newRenderSession.App.DefaultTheme, value: out themeModel);
        }

        string renderedTheme = themeModel == null || themeTemplate == null
            ? string.Empty
            : RenderTemplate(template: themeTemplate, model: themeModel, session: newRenderSession, pageReplacements: newReplacement.ToList());

        newReplacement.Add(item: new ReplacementDependency(old: "[theme[template]]", @new: renderedTheme));
        newReplacement.Add(item: new ReplacementDependency(old: "[theme[base]]", @new: baseTheme));
    }

    private string RenderTemplate(PageRenderTemplate template, object model, RenderSession session, IReadOnlyCollection<ReplacementDependency> pageReplacements)
    {
        List<ReplacementDependency> replacements = pageReplacements.ToList();
        replacements.Add(item: new ReplacementDependency(old: "[model]", @new: JsonConvert.SerializeObject(value: model)));
        replacements.AddRange(collection: BuildModelReplacements(model: model));

        return markupRenderService.RenderRenderSessionReplacementDependencies(
            key: template.ResourceKey,
            content: template.RawString,
            session: session,
            replacements: replacements,
            allowContentTags: false);
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

    private static string ResolveCulture(RenderSession session) =>
        !string.IsNullOrWhiteSpace(value: session.Request.Culture)
            ? session.Request.Culture
            : session.App?.DefaultCulture ?? string.Empty;

}