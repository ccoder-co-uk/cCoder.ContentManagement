// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections;
using System.Net;
using System.Reflection;
using System.Text.Json;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Models.PageRendering;
using cCoder.ContentManagement.Services.Foundations;
using cCoder.ContentManagement.Rendering.Services.Foundations;

namespace cCoder.ContentManagement.Rendering.Services.Processings;

internal sealed partial class MarkupRenderProcessingService(
    IMarkupRenderService markupRenderService,
    IJsonBroker jsonBroker) : IMarkupRenderProcessingService
{
    public RenderSession RenderRenderSession(RenderSession renderSession) =>
        TryCatch<RenderSession>(operation: () =>
    {
        ValidateRenderRenderSession(inputs: [renderSession]);
        ValidateRenderSession(session: renderSession);

        string key = string.IsNullOrWhiteSpace(value: renderSession.Target?.ResourceKey)
            ? "Default"
            : renderSession.Target.ResourceKey;

        List<MarkupReplacement> replacements = BuildDefaultReplacements(session: renderSession)
            .ToList();

        AddThemeTemplateReplacements(newRenderSession: renderSession, newReplacement: replacements);

        renderSession.Output = new RenderOutput
        {
            HeaderMarkup = markupRenderService.MarkContentSecurityPolicyNonce(
                markup: RenderMarkup(
                    key: key,
                    content: renderSession.Target?.HeaderMarkup ?? string.Empty,
                    session: renderSession,
                    replacements: replacements,
                    allowContentTags: renderSession.Target?.AllowHeaderContentTags ?? false)),
            BodyMarkup = renderSession.Request.HeaderOnly
                ? string.Empty
                : markupRenderService.MarkContentSecurityPolicyNonce(
                    markup: RenderMarkup(
                        key: key,
                        content: renderSession.Target?.BodyMarkup ?? string.Empty,
                        session: renderSession,
                        replacements: replacements,
                        allowContentTags: renderSession.Target?.AllowBodyContentTags ?? true))
        };

        return renderSession;

    });

    private IEnumerable<MarkupReplacement> BuildDefaultReplacements(RenderSession session)
    {
        string culture = ResolveCulture(session: session);

        string port = session.Config?.SslPort is int sslPort
            ? $":{sslPort}"
            : string.Empty;

        PageRenderUser user = session.User ?? new PageRenderUser();
        bool isGuest = string.IsNullOrWhiteSpace(value: user.Id) || string.Equals(a: user.Id, b: "Guest", comparisonType: StringComparison.OrdinalIgnoreCase);
        bool cacheTemplate = session.Request.CacheTemplate;

        List<MarkupReplacement> replacements =
        [
            new MarkupReplacement { Old = "[[user]]", Value = cacheTemplate
                ? PageRenderRuntimeTokens.User
                : jsonBroker.Serialize(value: new
            {
                Id = isGuest ? "Guest" : user.Id,
                DefaultCultureId = string.IsNullOrWhiteSpace(value: user.DefaultCultureId) ? culture : user.DefaultCultureId,
                DisplayName = isGuest ? "Guest" : user.DisplayName,
                Email = user.Email ?? string.Empty
            }) },
            new MarkupReplacement { Old = "[[displayname]]", Value = cacheTemplate
                ? PageRenderRuntimeTokens.DisplayName
                : isGuest ? "Guest" : user.DisplayName },
            new MarkupReplacement { Old = "[[loginlink]]", Value = cacheTemplate
                ? PageRenderRuntimeTokens.LoginLink
                : isGuest ? "<a href='/Login'>[resource_displayname[Login]]</a>" : "<a name='logout' href=''>[resource_displayname[Logout]]</a>" },
            new MarkupReplacement { Old = "[[date]]", Value = cacheTemplate
                ? PageRenderRuntimeTokens.Date
                : DateTimeOffset.UtcNow.ToString(format: "dd MMM yyyy") },
            new MarkupReplacement { Old = "[[culture]]", Value = WebUtility.HtmlEncode(value: culture) },
            new MarkupReplacement { Old = "[[lang]]", Value = WebUtility.HtmlEncode(value: culture.Split(separator: '-')
            .FirstOrDefault() ?? string.Empty) }
        ];

        if (session.App != null)
        {
            replacements.AddRange(collection:
            [
                new MarkupReplacement { Old = "[app[name]]", Value = session.App.Name ?? string.Empty },
                new MarkupReplacement { Old = "[app[domain]]", Value = session.App.Domain ?? string.Empty },
                new MarkupReplacement { Old = "[app[root]]", Value = "https://" + session.App.Domain + port + "/" },
                new MarkupReplacement { Old = "[app[id]]", Value = session.App.Id.ToString() },
                new MarkupReplacement { Old = "[api[root]]", Value = "https://" + session.App.Domain + port + "/Api/" }
            ]);
        }

        if (session.Page != null)
        {
            string pageRoot = session.App == null
                ? string.Empty
                : "https://" + session.App.Domain + port + "/";

            replacements.AddRange(collection:
            [
                new MarkupReplacement { Old = "[page[title]]", Value = session.Page.Title ?? string.Empty },
                new MarkupReplacement { Old = "[page[description]]", Value = session.Page.Description ?? string.Empty },
                new MarkupReplacement { Old = "[page[keywords]]", Value = session.Page.Keywords ?? string.Empty },
                new MarkupReplacement { Old = "[page[id]]", Value = session.Page.Id.ToString() },
                new MarkupReplacement { Old = "[page[parentid]]", Value = session.Page.ParentId?.ToString() ?? string.Empty },
                new MarkupReplacement { Old = "[page[path]]", Value = WebUtility.HtmlEncode(value: session.Page.Path ?? string.Empty) },
                new MarkupReplacement { Old = "[page[url]]", Value = WebUtility.HtmlEncode(value: pageRoot + (session.Page.Path ?? string.Empty)) },
                new MarkupReplacement
                {
                    Old = "[[editlink]]",
                    Value = CanPageRenderUser(
                        user: user,
                        appId: session.App?.Id,
                        operation: "page_update")
                            ? "<a href='?edit=true'>Edit</a>"
                            : string.Empty
                }
            ]);
        }

        if (!string.IsNullOrWhiteSpace(value: session.Request.Theme))
        {
            replacements.Add(item: new MarkupReplacement { Old = "[theme[name]]", Value = WebUtility.HtmlEncode(value: session.Request.Theme) });
        }

        if (session.Target?.Model != null)
        {
            replacements.Add(item: new MarkupReplacement { Old = "[model]", Value = jsonBroker.Serialize(value: session.Target.Model) });

            replacements.AddRange(collection: BuildModelReplacements(
                model: session.Target.Model));
        }

        replacements.AddRange(collection: BuildConfiguredReplacements(session: session));
        replacements.AddRange(collection: BuildThemeValueReplacements(session: session));

        return replacements;
    }

    private static IEnumerable<MarkupReplacement> BuildConfiguredReplacements(RenderSession session)
    {
        if (!string.IsNullOrWhiteSpace(
            value: session.Config?.WorkflowServiceUrl))
        {
            yield return new MarkupReplacement { Old = "[api[workflow]]", Value = session.Config.WorkflowServiceUrl };
        }
    }

    private IEnumerable<MarkupReplacement> BuildThemeValueReplacements(RenderSession session)
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
            foreach (MarkupReplacement replacement in BuildThemeReplacements(model: requestedTheme))
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

        foreach (MarkupReplacement replacement in BuildThemeReplacements(model: requestedTheme))
        {
            yield return replacement;
        }
    }

    private void AddThemeTemplateReplacements(RenderSession newRenderSession, ICollection<MarkupReplacement> newReplacement)
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

        newReplacement.Add(item: new MarkupReplacement { Old = "[theme[template]]", Value = renderedTheme });
        newReplacement.Add(item: new MarkupReplacement { Old = "[theme[base]]", Value = baseTheme });
    }

    private string RenderTemplate(PageRenderTemplate template, object model, RenderSession session, IReadOnlyCollection<MarkupReplacement> pageReplacements)
    {
        List<MarkupReplacement> replacements = pageReplacements.ToList();
        replacements.Add(item: new MarkupReplacement { Old = "[model]", Value = jsonBroker.Serialize(value: model) });
        replacements.AddRange(collection: BuildModelReplacements(model: model));

        return RenderMarkup(
            key: template.ResourceKey,
            content: template.RawString,
            session: session,
            replacements: replacements,
            allowContentTags: false);
    }

    internal string RenderMarkup(
        string key,
        string content,
        RenderSession session,
        IReadOnlyCollection<MarkupReplacement> replacements,
        bool allowContentTags)
    {
        if (string.IsNullOrEmpty(value: content))
        {
            return string.Empty;
        }

        TagHandlingOperation operation = RenderTagHandlingOperation(
            operation: new TagHandlingOperation
            {
                Session = session,
                ResourceKey = key,
                Content = content,
                AllowContentTags = allowContentTags,
                Editable = session.Request.Edit,
                Replacements = replacements,
                Fragments = []
            });

        return operation.Content;
    }

    private TagHandlingOperation RenderTagHandlingOperation(
        TagHandlingOperation operation)
    {
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

            operation = markupRenderService
                .RenderCultureLinkTagHandlingOperation(
                    tagHandlingOperation: operation);

            operation = markupRenderService
                .RenderMetadataTagHandlingOperation(
                    tagHandlingOperation: operation);

            operation = markupRenderService
                .RenderNavigationTagHandlingOperation(
                    tagHandlingOperation: operation);

            operation = markupRenderService
                .RenderContentTagHandlingOperation(
                    tagHandlingOperation: operation);

            operation = markupRenderService
                .RenderComponentTagHandlingOperation(
                    tagHandlingOperation: operation);

            operation = markupRenderService
                .RenderScriptTagHandlingOperation(
                    tagHandlingOperation: operation);

            operation = markupRenderService
                .RenderStyleTagHandlingOperation(
                    tagHandlingOperation: operation);

            operation = RenderReplacementTagHandlingOperation(
                operation: operation);

            operation = markupRenderService
                .RenderDmsTagHandlingOperation(
                    tagHandlingOperation: operation);

            operation = markupRenderService
                .RenderResourceTagHandlingOperation(
                    tagHandlingOperation: operation);

            operation = markupRenderService
                .RenderExecuteTagHandlingOperation(
                    tagHandlingOperation: operation);

            foreach (TagHandlingFragment fragment in operation.Fragments)
            {
                TagHandlingOperation renderedFragment =
                    RenderTagHandlingOperation(operation: fragment.Operation);

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

    private static TagHandlingOperation RenderReplacementTagHandlingOperation(
        TagHandlingOperation operation)
    {
        foreach (MarkupReplacement replacement in operation.Replacements)
        {
            operation.Content = operation.Content.Replace(
                oldValue: replacement.Old,
                newValue: replacement.New);
        }

        return operation;
    }

    private IEnumerable<MarkupReplacement> BuildModelReplacements(object model, string prefix = "")
    {
        if (model == null)
        {
            return Array.Empty<MarkupReplacement>();
        }

        if (model is string text)
        {
            return [new MarkupReplacement { Old = "[model[" + prefix + "]]", Value = text }];
        }

        if (model is JsonElement jsonElement)
        {
            return BuildModelReplacements(
                model: jsonBroker.ParseJson(json: jsonElement.GetRawText()),
                prefix: prefix);
        }

        if (jsonBroker.IsJsonObject(value: model))
        {
            return BuildJObjectReplacements(model: model, prefix: prefix);
        }

        if (jsonBroker.IsJsonArray(value: model))
        {
            return BuildCollectionReplacements(model: (IEnumerable)model, prefix: prefix);
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

    private IEnumerable<MarkupReplacement> BuildCollectionReplacements(IEnumerable model, string prefix)
    {
        List<MarkupReplacement> replacements = [];
        int index = 0;

        foreach (object item in model)
        {
            replacements.AddRange(collection: BuildModelReplacements(model: item, prefix: $"{prefix}[{index}]"));
            index++;
        }

        string lengthBinding = string.IsNullOrEmpty(value: prefix) ? "Length" : prefix + ".Length";
        replacements.Add(item: new MarkupReplacement { Old = "[model[" + lengthBinding + "]]", Value = index.ToString() });

        return replacements;
    }

    private IEnumerable<MarkupReplacement> BuildObjectReplacements(object model, string prefix) =>
        model.GetType()
        .GetProperties()
        .SelectMany(selector: property =>
            {
                object value = property.GetValue(obj: model);
                string bindingExpression = string.IsNullOrEmpty(value: prefix) ? property.Name : prefix + "." + property.Name;

                if (property.PropertyType.IsValueType || property.PropertyType == typeof(string))
                {
                    return [new MarkupReplacement { Old = "[model[" + bindingExpression + "]]", Value = value?.ToString() ?? string.Empty }];
                }

                return value != null
                    ? BuildModelReplacements(model: value, prefix: bindingExpression)
                    : Array.Empty<MarkupReplacement>();
            });

    private IEnumerable<MarkupReplacement> BuildJObjectReplacements(object model, string prefix) =>
        jsonBroker.GetJsonProperties(value: model)
        .SelectMany(selector: property =>
        {
            string bindingExpression = string.IsNullOrEmpty(value: prefix) ? property.Key : prefix + "." + property.Key;

            return jsonBroker.IsJsonValue(value: property.Value)
                ? [new MarkupReplacement { Old = "[model[" + bindingExpression + "]]", Value = property.Value.ToString() }]
                : BuildModelReplacements(model: property.Value, prefix: bindingExpression);
        });

    private IEnumerable<MarkupReplacement> BuildDynamicReplacements(IDictionary<string, object> model, string prefix) =>
        model.Keys.SelectMany(selector: key =>
                                                                                                                           {
                                                                                                                               string bindingExpression = string.IsNullOrEmpty(value: prefix) ? key : prefix + "." + key;
                                                                                                                               object value = model[key];

                                                                                                                               List<MarkupReplacement> replacements = [new MarkupReplacement { Old = "[model[" + bindingExpression + "]]", Value = value?.ToString() ?? string.Empty }];

                                                                                                                               if (value != null && !value.GetType()
                                                                                                                                   .IsValueType && value is not string)
                                                                                                                               {
                                                                                                                                   replacements.AddRange(collection: BuildModelReplacements(model: value, prefix: bindingExpression));
                                                                                                                               }

                                                                                                                               return replacements;
                                                                                                                           });

    private IEnumerable<MarkupReplacement> BuildThemeReplacements(object model, string prefix = "")
    {
        if (model == null)
        {
            return Array.Empty<MarkupReplacement>();
        }

        if (jsonBroker.IsJsonObject(value: model))
        {
            return BuildThemeJObjectReplacements(model: model, prefix: prefix);
        }

        if (model is string text)
        {
            return [new MarkupReplacement { Old = "[theme[" + prefix + "]]", Value = text }];
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

    private IEnumerable<MarkupReplacement> BuildThemeCollectionReplacements(IEnumerable model, string prefix)
    {
        string bindingExpression = prefix ?? string.Empty;
        List<MarkupReplacement> replacements = [];
        int index = 0;

        foreach (object item in model)
        {
            replacements.AddRange(collection: BuildThemeReplacements(model: item, prefix: bindingExpression + $"[{index}]"));
            index++;
        }

        string lengthBinding = bindingExpression.Length == 0 ? "Length" : bindingExpression + ".Length";
        replacements.Add(item: new MarkupReplacement { Old = "[theme[" + lengthBinding + "]]", Value = index.ToString() });

        return replacements;
    }

    private IEnumerable<MarkupReplacement> BuildThemeObjectReplacements(object model, string prefix) =>
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
                        new MarkupReplacement { Old = "[theme[" + prefix + "]]", Value = model?.ToString() ?? string.Empty },
                        new MarkupReplacement { Old = "[theme[" + bindingExpression + "]]", Value = value?.ToString() ?? string.Empty }
                    ];
                }

                return value != null
                    ? BuildThemeReplacements(model: value, prefix: bindingExpression)
                    : Array.Empty<MarkupReplacement>();
            });

    private IEnumerable<MarkupReplacement> BuildThemeJObjectReplacements(object model, string prefix) =>
        jsonBroker.GetJsonProperties(value: model)
        .SelectMany(selector: property =>
        {
            string bindingExpression = string.IsNullOrEmpty(value: prefix) ? property.Key : prefix + "." + property.Key;

            return jsonBroker.IsJsonValue(value: property.Value)
                ? [new MarkupReplacement { Old = "[theme[" + bindingExpression + "]]", Value = property.Value.ToString() }]
                : BuildThemeReplacements(model: property.Value, prefix: bindingExpression);
        });

    private IEnumerable<MarkupReplacement> BuildThemeDynamicReplacements(IDictionary<string, object> model, string prefix) =>
        model.Keys.SelectMany(selector: key =>
                                                                                                                                {
                                                                                                                                    string bindingExpression = string.IsNullOrEmpty(value: prefix) ? key : prefix + "." + key;
                                                                                                                                    object value = model[key];

                                                                                                                                    List<MarkupReplacement> replacements = [new MarkupReplacement { Old = "[theme[" + bindingExpression + "]]", Value = value?.ToString() ?? string.Empty }];

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