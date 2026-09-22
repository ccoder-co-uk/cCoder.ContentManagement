// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Reflection;
using System.Text;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Models.PageRendering;
using cCoder.ContentManagement.Models.RegularExpressions;
using cCoder.ContentManagement.Models.Rendering;
using cCoder.ContentManagement.Services.Foundations.Rendering;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class TemplateRenderProcessingService(
    ITemplateRenderService templateRenderService,
    ContentManagementConfiguration config = null)
        : ITemplateRenderProcessingService
{
    private const string TagPattern = "\\[TYPE\\[[A-Za-z\\d_/-]*\\][A-Za-z\\d_/-]*\\=*\\\"*-*[A-Za-z\\d_/-]*\\\"*\\]";

    public string RenderTemplateRenderOperation(
        TemplateRenderOperation templateRenderOperation) =>
        TryCatch<string>(operation: () =>
    {
        ValidateRenderTemplateOperation(inputs: [templateRenderOperation]);

        templateRenderOperation.Result = templateRenderOperation.Template != null
            ? RenderTemplateRenderParams(
                template: templateRenderOperation.Template,
                model: templateRenderOperation.Model,
                renderParams: templateRenderOperation.RenderParams)
            : RenderUser(
                appId: templateRenderOperation.AppId,
                name: templateRenderOperation.Name,
                model: templateRenderOperation.Model,
                user: templateRenderOperation.User,
                culture: templateRenderOperation.Culture);

        return templateRenderOperation.Result;
    });

    internal string RenderUser(
        int appId,
        string name,
        object model,
        User user,
        string culture) =>
        TryCatch<string>(operation: () =>
    {
        ValidateRenderUser(inputs: [appId, name, model, user, culture]);
        ValidateAppId(appId: appId, parameterName: "appId");
        ValidateTemplateName(name: name, parameterName: "name");
        ValidateModel(model: model, parameterName: "model");
        ValidateUser(user: user, parameterName: "user");

        App app = null;

        foreach (App existingApp in GetApps())
        {
            if (existingApp.Id != appId)
            {
                continue;
            }

            app = new App
            {
                Id = existingApp.Id,
                DefaultCultureId = existingApp.DefaultCultureId,
                TenantId = existingApp.TenantId,
                Name = existingApp.Name,
                Domain = existingApp.Domain,
                DefaultTheme = existingApp.DefaultTheme,
                ConfigJson = existingApp.ConfigJson
            };

            break;
        }

        if (app == null)
        {
            throw new InvalidOperationException(message: $"App '{appId}' was not found.");
        }

        List<Component> components = [];
        List<Resource> resources = [];
        List<Script> scripts = [];

        foreach (Component existingComponent in GetComponents())
        {
            if (existingComponent.AppId == appId)
            {
                components.Add(item: existingComponent);
            }
        }

        foreach (Resource existingResource in GetResources())
        {
            if (existingResource.AppId == appId)
            {
                resources.Add(item: existingResource);
            }
        }

        foreach (Script existingScript in GetScripts())
        {
            if (existingScript.AppId == appId)
            {
                scripts.Add(item: existingScript);
            }
        }

        app.Components = components;
        app.Resources = resources;
        app.Scripts = scripts;

        Template template = FindFirst(
            source: GetTemplates(),
            predicate: existingTemplate =>
                existingTemplate.AppId == appId
                && existingTemplate.Name.Equals(value: name, comparisonType: StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException(message: "Template '" + name + "' was not found.");

        TemplateRenderParams templateRenderParams = new()
        {
            App = app,
            User = user,
            Culture = culture
        };

        return ExecuteRenderTemplateRenderParamsConfig(
            template: template,
            model: model,
            renderParams: templateRenderParams,
            config: config);

    });

    internal string RenderTemplateRenderParams(
        Template template,
        object model,
        RenderParams renderParams) =>
        TryCatch<string>(operation: () =>
    {
        ValidateRenderTemplateRenderParams(inputs: [template, model, renderParams]);
        ValidateTemplate(template: template, parameterName: "template");
        ValidateModel(model: model, parameterName: "model");
        ValidateRenderParamsArgument(renderParams: renderParams, parameterName: "renderParams");

        List<MarkupReplacement> list = CopyToList(
            source: DefaultReplacements(renderParams: renderParams, config: config));

        list.Add(item: new MarkupReplacement { Old = "[model]", Value = Serialize(value: model) });
        list.AddRange(collection: BuildModelReplacements(model: model));

        if (IsLoggingEnabled(logLevel: LogLevel.Debug))
        {
            LogDebug(
                message: "Rendering template {Template} with {ReplacementCount} replacements.",
                args: [template.Name, list.Count]);
        }

        return ProcessContentString(key: template.ResourceKey, renderParams: renderParams, content: template.RawString, replacements: list);

    });

    private ICollection<MarkupReplacement> DefaultReplacements(
        RenderParams renderParams,
        ContentManagementConfiguration config = null)
    {
        ValidateRenderParams(renderParams: renderParams, replacements: null);

        if (renderParams.Culture == null)
        {
            string text = (renderParams.Culture = string.Empty);
        }

        string text2 = (string.IsNullOrEmpty(value: renderParams.Culture) ? renderParams.App.DefaultCultureId : renderParams.Culture);

        string text3 = config?.SslPort is int sslPort
            ? $":{sslPort}"
            : string.Empty;

        int cultureSeparator = text2.IndexOf(value: '-');
        string language = cultureSeparator < 0 ? text2 : text2[..cultureSeparator];

        List<MarkupReplacement> list =
        [
            new MarkupReplacement
        {
            Old = "[[user]]",
            Value = Serialize(value: new
            {
                Id = renderParams.User?.Id,
                DefaultCultureId = renderParams.User?.DefaultCultureId,
                DisplayName = renderParams.User?.DisplayName,
                Email = renderParams.User?.Email
            })
        },
            new MarkupReplacement { Old = "[[displayname]]", Value = renderParams.User?.DisplayName },
            new MarkupReplacement { Old = "[[loginlink]]", Value = (renderParams.User?.Id == "Guest") ? "<a href='/Login'>[resource_displayname[Login]]</a>" : "<a name='logout' href=''>[resource_displayname[Logout]]</a>" },
            new MarkupReplacement { Old = "[[date]]", Value = DateTimeOffset.UtcNow.ToString(format: "dd MMM yyyy") },
            new MarkupReplacement { Old = "[[culture]]", Value = text2 },
            new MarkupReplacement { Old = "[[lang]]", Value = language },
            new MarkupReplacement { Old = "[app[name]]", Value = renderParams.App?.Name },
            new MarkupReplacement { Old = "[app[domain]]", Value = renderParams.App?.Domain },
            new MarkupReplacement { Old = "[app[root]]", Value = "https://" + renderParams.App?.Domain + text3 + "/" },
            new MarkupReplacement { Old = "[app[id]]", Value = renderParams.App?.Id.ToString() }
        ];

        List<MarkupReplacement> list2 = list;

        if (config != null)
        {
            if (!string.IsNullOrWhiteSpace(value: config.WorkflowServiceUrl))
            {
                list2.Add(item: new MarkupReplacement { Old = "[api[workflow]]", Value = config.WorkflowServiceUrl });
            }

            list2.Add(item: new MarkupReplacement { Old = "[api[root]]", Value = "https://" + renderParams.App?.Domain + text3 + "/Api/" });
        }

        if (renderParams is TemplateRenderParams)
        {
            list2.Add(item: new MarkupReplacement { Old = "[theme[name]]", Value = "Default" });
            IDictionary<string, object> source = default(IDictionary<string, object>);
            object firstTheme = null;

            if (TryGetThemeDictionary(config: renderParams.App.Config, themeDictionary: out source)
                && TryGetFirstValue(source: source, value: out firstTheme))
            {
                list2.AddRange(collection: BuildThemeReplacements(model: firstTheme));
            }
        }

        return list2;
    }

    private string ProcessContentString(string key, RenderParams renderParams, string content, IEnumerable<MarkupReplacement> replacements)
    {
        if (content == null)
        {
            return string.Empty;
        }

        if (key == null)
        {
            key = "Default";
        }

        if (renderParams.Culture == null)
        {
            string text = (renderParams.Culture = string.Empty);
        }

        ValidateRenderParams(renderParams: renderParams, replacements: replacements);
        StringBuilder result = new StringBuilder(value: content, capacity: content.Length * 4);
        Script(key: key, source: result, renderParams: renderParams, replacements: replacements);
        RegexReplace(source: result, matchExpression: "\\[TYPE\\[[A-Za-z\\d_/-]*\\][A-Za-z\\d_/-]*\\=*\\\"*-*[A-Za-z\\d_/-]*\\\"*\\]".Replace(oldValue: "TYPE", newValue: "culturelink"), action: match => "?culture=" + GetTagName(source: match));
        Component(key: key, renderParams: renderParams, replacements: replacements, result: result);
        Meta(source: result, culture: renderParams.Culture);
        Resource(key: key, source: result, renderParams: renderParams, replacements: replacements);
        ExecuteAsync(key: key, source: result, renderParams: renderParams, replacements: replacements);

        foreach (MarkupReplacement replacement in replacements)
        {
            result.Replace(oldValue: replacement.Old, newValue: replacement.New);
        }

        return result.ToString();
    }

    private static void ValidateRenderParams(RenderParams renderParams, IEnumerable<MarkupReplacement> replacements)
    {
        if (renderParams == null)
        {
            throw new ValidationException(message: "renderParams is required.");
        }

        if (renderParams.App == null)
        {
            throw new ValidationException(message: "renderParams.App is required.");
        }

        if (renderParams.App.Resources == null)
        {
            throw new ValidationException(message: "renderParams.App.Resources is required.");
        }

        if (replacements != null)
        {
        }
    }

    private static (string type, string name, string[] options) SplitMatch(
        RegularExpressionMatch match)
    {
        string[] array = match.Value
            .Split(separator: "[");

        string[] array2 = array[^1]
            .Split(separator: "]");

        return (type: array[1].ToLower(), name: array2[0].ToLower(), options: array2[1].Split(separator: "|", options: StringSplitOptions.RemoveEmptyEntries));
    }

    private void Script(string key, StringBuilder source, RenderParams renderParams, IEnumerable<MarkupReplacement> replacements) =>
        RegexReplace(source: source, matchExpression: "\\[script\\[[A-Za-z\\d_/. \\-]*\\]\\]", action: match =>
                                                                                                                               {
                                                                                                                                   string name = match.Value.Replace(oldValue: "[script[", newValue: "")
                                                                                                                                       .Replace(oldValue: "]]", newValue: "")
                                                                                                                                       .ToLower();

                                                                                                                                   Script script = GetScript(key: "script|" + name);

                                                                                                                                   if (script != null)
                                                                                                                                   {
                                                                                                                                        Script obj = FindFirst(source: renderParams.App?.Scripts, predicate: s => s.Name.Equals(value: name, comparisonType: StringComparison.CurrentCultureIgnoreCase));
                                                                                                                                       return ProcessContentString(key: key, renderParams: renderParams, content: obj?.Content ?? script.Content, replacements: replacements);
                                                                                                                                   }

                                                                                                                                   return string.Empty;
                                                                                                                               });

    private void Component(string key, RenderParams renderParams, IEnumerable<MarkupReplacement> replacements, StringBuilder result) =>
        RegexReplace(source: result, matchExpression: "\\[TYPE\\[[A-Za-z\\d_/-]*\\][A-Za-z\\d_/-]*\\=*\\\"*-*[A-Za-z\\d_/-]*\\\"*\\]".Replace(oldValue: "TYPE", newValue: "component"), action: match =>
                                                                                                                                  {
                                                                                                                                      (string _, string name, string[] options) tag = SplitMatch(match: match);
                                                                                                                                      Component component = FindFirst(source: renderParams.App?.Components, predicate: c => c.Name.Equals(value: tag.name, comparisonType: StringComparison.CurrentCultureIgnoreCase)) ?? GetComponent(key: "component|" + tag.name);
                                                                                                                                      return (component == null) ? ("[[Missing Component:" + tag.name + "]]") : ProcessContentString(key: key, renderParams: renderParams, content: BuildComponentMarkup(component: component, tag: tag, replacements: replacements, renderParams: renderParams), replacements: replacements);
                                                                                                                                  });

    private string BuildComponentMarkup(Component component, (string type, string name, string[] options) tag, IEnumerable<MarkupReplacement> replacements, RenderParams renderParams)
    {
        List<string> classes = [];
        List<string> attributes = [];

        foreach (string option in tag.options)
        {
            if (option.StartsWith(value: "class="))
            {
                classes.Add(item: option.Replace(oldValue: "class=", newValue: ""));
            }
            else
            {
                attributes.Add(item: option);
            }
        }

        string value = string.Join(separator: " ", values: classes);
        string content = $"<section name='{component.Name}' class='component {value}' data-id='{component.Id}' data-resource-key='{component.ResourceKey}' {string.Join(separator: " ", values: attributes)}>\r\n                        {ProcessContentString(key: component.ResourceKey, renderParams: renderParams, content: component.Content, replacements: replacements)}\r\n                        <script type='text/javascript' nonce='{ContentSecurityPolicyNonceContract.Placeholder}'>{ProcessContentString(key: component.ResourceKey, renderParams: renderParams, content: component.Script, replacements: replacements)}</script>\r\n                    </section>";
        return ProcessContentString(key: component.ResourceKey, renderParams: renderParams, content: content, replacements: replacements);
    }

    private void ExecuteAsync(string key, StringBuilder source, RenderParams renderParams, IEnumerable<MarkupReplacement> replacements) =>
        RegexReplace(source: source, matchExpression: "\\[execute\\](.*?)\\[/execute\\]", action: match =>
                                                                                                                                     {
                                                                                                                                         string value = match.Groups["1"];

                                                                                                                                         string content = SerializeIgnoringReferences(value: new
                                                                                                                                         {
                                                                                                                                             Script = value,
                                                                                                                                             Model = ParseJson(json: FindRequired(source: replacements, predicate: r => r.Old == "[model]")
                                                                                                                                             .New)
                                                                                                                                         });

                                                                                                                                         string result = ExecuteWorkflow(
                                                                                                                                             baseAddress: FindRequired(source: replacements, predicate: replacement => replacement.Old == "[api[workflow]]").New,
                                                                                                                                             content: content);

                                                                                                                                         return ProcessContentString(key: key, renderParams: renderParams, content: result, replacements: replacements);
                                                                                                                                     });

    private void Resource(string key, StringBuilder source, RenderParams renderParams, IEnumerable<MarkupReplacement> replacements)
    {
        List<Resource> known = new List<Resource>();
        List<string> namesInKey = new List<string>();

        RegexMatch(source: source, matchExpression: "\\[TYPE\\[[A-Za-z\\d_/-]*\\][A-Za-z\\d_/-]*\\=*\\\"*-*[A-Za-z\\d_/-]*\\\"*\\]".Replace(oldValue: "TYPE", newValue: "resource_displayname"), action: match =>
        {
            namesInKey.Add(item: GetTagName(source: match));
        });

        RegexMatch(source: source, matchExpression: "\\[TYPE\\[[A-Za-z\\d_/-]*\\][A-Za-z\\d_/-]*\\=*\\\"*-*[A-Za-z\\d_/-]*\\\"*\\]".Replace(oldValue: "TYPE", newValue: "resource_shortdisplayname"), action: match =>
        {
            namesInKey.Add(item: GetTagName(source: match));
        });

        RegexMatch(source: source, matchExpression: "\\[TYPE\\[[A-Za-z\\d_/-]*\\][A-Za-z\\d_/-]*\\=*\\\"*-*[A-Za-z\\d_/-]*\\\"*\\]".Replace(oldValue: "TYPE", newValue: "resource_description"), action: match =>
        {
            namesInKey.Add(item: GetTagName(source: match));
        });

        if (namesInKey.Count == 0)
        {
            return;
        }

        List<Resource> list = known;
        IEnumerable<Resource> collection;

        if (renderParams.App.Resources != null)
        {
            List<Resource> list2 = new List<Resource>();
            list2.AddRange(collection: SelectResourcesForCulture(potentials: renderParams.App.Resources, key: key, culture: renderParams.Culture ?? string.Empty));
            IEnumerable<Resource> enumerable = list2;
            collection = enumerable;
        }
        else
        {
            IEnumerable<Resource> enumerable = Array.Empty<Resource>();
            collection = enumerable;
        }

        list.AddRange(collection: collection);
        string key2 = key.ToLowerInvariant();
        string culture = renderParams.Culture.ToLowerInvariant();

        foreach (string item in namesInKey)
        {
            Resource resource = FindResourceInCache(key: key2, name: item.ToLowerInvariant(), culture: culture);

            if (resource != null)
            {
                known.Add(item: resource);
            }
        }

        RegexReplace(source: source, matchExpression: "\\[TYPE\\[[A-Za-z\\d_/-]*\\][A-Za-z\\d_/-]*\\=*\\\"*-*[A-Za-z\\d_/-]*\\\"*\\]".Replace(oldValue: "TYPE", newValue: "resource_displayname"), action: match => ProcessContentString(key: key, renderParams: renderParams, content: FindFirst(source: known, predicate: resource => resource.Name.Equals(value: GetTagName(source: match), comparisonType: StringComparison.CurrentCultureIgnoreCase))?.DisplayName ?? GetTagName(source: match)
            .ToLower(), replacements: replacements));

        RegexReplace(source: source, matchExpression: "\\[TYPE\\[[A-Za-z\\d_/-]*\\][A-Za-z\\d_/-]*\\=*\\\"*-*[A-Za-z\\d_/-]*\\\"*\\]".Replace(oldValue: "TYPE", newValue: "resource_shortdisplayname"), action: match => ProcessContentString(key: key, renderParams: renderParams, content: FindFirst(source: known, predicate: resource => resource.Name.Equals(value: GetTagName(source: match), comparisonType: StringComparison.CurrentCultureIgnoreCase))?.ShortDisplayName ?? GetTagName(source: match)
            .ToLower(), replacements: replacements));

        RegexReplace(source: source, matchExpression: "\\[TYPE\\[[A-Za-z\\d_/-]*\\][A-Za-z\\d_/-]*\\=*\\\"*-*[A-Za-z\\d_/-]*\\\"*\\]".Replace(oldValue: "TYPE", newValue: "resource_description"), action: match => ProcessContentString(key: key, renderParams: renderParams, content: FindFirst(source: known, predicate: resource => resource.Name.Equals(value: GetTagName(source: match), comparisonType: StringComparison.CurrentCultureIgnoreCase))?.Description ?? GetTagName(source: match)
            .ToLower(), replacements: replacements));
    }

    private Resource FindResourceInCache(string key, string name, string culture)
    {
        Resource resource = FindResourceInCacheForKey(
            key: key,
            name: name,
            culture: culture);

        return resource
            ?? (string.Equals(
                a: key,
                b: "default",
                comparisonType: StringComparison.OrdinalIgnoreCase)
                    ? null
                    : FindResourceInCacheForKey(
                        key: "default",
                        name: name,
                        culture: culture));
    }

    private Resource FindResourceInCacheForKey(
        string key,
        string name,
        string culture)
    {
        Resource resource = GetResource(key: $"resource|{key}-{name}-{culture}");

        if (resource != null)
        {
            return resource;
        }

        if (culture.Contains(value: '-'))
        {
            string value = culture.Split(separator: "-")[0];
            Resource resource2 = GetResource(key: $"resource|{key}-{name}-{value}");

            if (resource2 != null)
            {
                return resource2;
            }
        }

        return GetResource(key: $"resource|{key}-{name}-{string.Empty}");
    }

    private static IEnumerable<Resource> SelectResourcesForCulture(IEnumerable<Resource> potentials, string key, string culture)
    {
        List<Resource> list = new List<Resource>();

        Dictionary<string, List<Resource>> resourcesByName =
            new(comparer: StringComparer.OrdinalIgnoreCase);

        foreach (Resource resource in potentials ?? [])
        {
            if (!string.Equals(
                a: resource.Key,
                b: key,
                comparisonType: StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            string name = resource.Name ?? string.Empty;

            if (!resourcesByName.TryGetValue(key: name, value: out List<Resource> resources))
            {
                resources = [];
                resourcesByName.Add(key: name, value: resources);
            }

            resources.Add(item: resource);
        }

        foreach (List<Resource> resources in resourcesByName.Values)
        {
            Resource closestCulturalMatch = GetClosestCulturalMatch(
                potentials: resources,
                culture: culture);

            if (closestCulturalMatch != null)
            {
                list.Add(item: closestCulturalMatch);
            }
        }

        return list;
    }

    private static Resource GetClosestCulturalMatch(IEnumerable<Resource> potentials, string culture)
    {
        Resource resource = null;

        string[] cultureParts = (culture ?? string.Empty).ToLowerInvariant()
            .Split(separator: '-');

        int num = cultureParts.Length;
        string resultCulture = string.Empty;

        while (resource == null && resultCulture != null)
        {
            resultCulture = string.Join(separator: "-", value: cultureParts, startIndex: 0, count: num);

            resource = FindFirst(
                source: potentials,
                predicate: candidate => string.Equals(
                    a: candidate.Culture,
                    b: resultCulture,
                    comparisonType: StringComparison.OrdinalIgnoreCase));

            num--;

            if (num == 0)
            {
                resultCulture = null;
            }
        }

        return resource ?? FindFirst(
            source: potentials,
            predicate: candidate => string.IsNullOrEmpty(value: candidate.Culture));
    }

    private void Meta(StringBuilder source, string culture) =>
        RegexReplace(source: source, matchExpression: "\\[TYPE\\[[A-Za-z\\d_/-]*\\][A-Za-z\\d_/-]*\\=*\\\"*-*[A-Za-z\\d_/-]*\\\"*\\]".Replace(oldValue: "TYPE", newValue: "meta"), action: match =>
                                                               {
                                                                   string value = match.Value;
                                                                   string text = value.Substring(startIndex: 6, length: value.Length - 6);
                                                                   string key = text[..text.IndexOf(value: ']')].ToLowerInvariant();
                                                                   return GetMetadata(key: key, culture: culture);
                                                               });

    private static bool TryGetThemeDictionary(dynamic config, out IDictionary<string, object> themeDictionary)
    {
        themeDictionary = null;

        if (!(config is IDictionary<string, object> dictionary))
        {
            return false;
        }

        if (!dictionary.TryGetValue(key: "Themes", value: out var value))
        {
            return false;
        }

        themeDictionary = value as IDictionary<string, object>;
        return themeDictionary != null;
    }

    private IEnumerable<MarkupReplacement> BuildThemeReplacements<T>(T model, string prefix = "")
    {
        if ((object)model.GetType()
            .GetInterface(name: "IDynamicMetaObjectProvider") != null
            && !IsJsonObject(value: model))
        {
            return BuildDynamicThemeReplacements(model: model, prefix: prefix);
        }

        if (IsJsonObject(value: model))
        {
            return BuildJObjectThemeReplacements(model: model, prefix: prefix);
        }

        if (model is string)
        {
            return new[] { new MarkupReplacement { Old = "[theme[" + prefix + "]]", Value = model.ToString() } };
        }

        if (!(model is IEnumerable))
        {
            return BuildIEnumerableThemeReplacements(model: model, prefix: prefix);
        }

        return BuildObjectThemeReplacements(model: model, prefix: prefix);
    }

    private List<MarkupReplacement> BuildObjectThemeReplacements<T>(T model, string prefix)
    {
        string text = prefix ?? string.Empty;
        List<MarkupReplacement> list = new List<MarkupReplacement>();
        int num = 0;

        foreach (object item in (IEnumerable)(object)model)
        {
            string prefix2 = text + $"[{num}]";
            list.AddRange(collection: BuildThemeReplacements(model: item, prefix: prefix2));
            num++;
        }

        string text2 = ((text.Length == 0) ? "Length" : (text + ".Length"));
        list.Add(item: new MarkupReplacement { Old = "[theme[" + text2 + "]]", Value = num.ToString() });
        return list;
    }

    private IEnumerable<MarkupReplacement> BuildIEnumerableThemeReplacements<T>(T model, string prefix)
    {
        List<MarkupReplacement> replacements = [];

        IReadOnlyList<RuntimePropertyValue> properties = templateRenderService.GetPropertyValuesTemplateRenderFoundationOperation(
            templateRenderFoundationOperation: new TemplateRenderFoundationOperation
            {
                Value = model
            })
        .RuntimeProperties;

        foreach (RuntimePropertyValue property in properties)
        {
            object value = property.Value;
            string text = ((prefix.Length > 0) ? (prefix + "." + property.Name) : property.Name);

            if (property.IsValueType)
            {
                replacements.Add(item: new MarkupReplacement
                {
                    Old = "[theme[" + prefix + "]]",
                    Value = model?.ToString() ?? string.Empty
                });

                replacements.Add(item: new MarkupReplacement
                {
                    Old = "[theme[" + text + "]]",
                    Value = value?.ToString() ?? string.Empty
                });
            }
            else if (value != null)
            {
                replacements.AddRange(collection: BuildThemeReplacements(model: value, prefix: text));
            }
        }

        RemoveInvalidReplacements(replacements: replacements);
        return replacements;
    }

    private IEnumerable<MarkupReplacement> BuildJObjectThemeReplacements<T>(T model, string prefix)
    {
        IEnumerable<KeyValuePair<string, object>> source =
            GetJsonProperties(value: model);

        List<MarkupReplacement> replacements = [];

        foreach (KeyValuePair<string, object> token in source)
        {
            string text = ((prefix.Length > 0) ? (prefix + "." + token.Key) : token.Key);

            if (IsJsonValue(value: token.Value))
            {
                replacements.Add(item: new MarkupReplacement { Old = "[theme[" + text + "]]", Value = token.Value.ToString() ?? string.Empty });
            }
            else if (token.Value != null)
            {
                replacements.AddRange(collection: BuildThemeReplacements(model: token.Value, prefix: text));
            }

        }

        return replacements;
    }

    private IEnumerable<MarkupReplacement> BuildDynamicThemeReplacements<T>(T model, string prefix)
    {
        IDictionary<string, object> dynamicModel = (IDictionary<string, object>)(object)model;

        List<MarkupReplacement> replacements = [];

        foreach (string key in dynamicModel.Keys)
        {
            string text = ((prefix.Length > 0) ? (prefix + "." + key) : key);
            replacements.Add(item: new MarkupReplacement { Old = "[theme[" + text + "]]", Value = dynamicModel[key]?.ToString() ?? string.Empty });

            if (dynamicModel[key] != null && !dynamicModel[key].GetType()
                .IsValueType)
            {
                replacements.AddRange(collection: BuildThemeReplacements(model: dynamicModel[key], prefix: text));
            }

        }

        return replacements;
    }

    private IEnumerable<MarkupReplacement> BuildModelReplacements(object model, string prefix = "")
    {
        model = NormalizeJson(value: model);

        if (model is string)
        {
            return new[] { new MarkupReplacement { Old = "[theme[" + prefix + "]]", Value = model.ToString() } };
        }

        if (IsJsonObject(value: model))
        {
            return BuildModelReplacementsForJObject(model: model, prefix: prefix);
        }

        if (IsJsonArray(value: model))
        {
            return BuildModelReplacementsForCollection(model: model, prefix: prefix);
        }

        if ((object)model.GetType()
            .GetInterface(name: "IDynamicMetaObjectProvider") != null)
        {
            return BuildModelReplacementsForDynamicObject(model: model, prefix: prefix);
        }

        return (model is IEnumerable) ? BuildModelReplacementsForCollection(model: model, prefix: prefix) : BuildModelReplacementsForObject(model: model, prefix: prefix);
    }

    private IEnumerable<MarkupReplacement> BuildModelReplacementsForCollection(object model, string prefix)
    {
        string text = prefix ?? string.Empty;
        List<MarkupReplacement> list = new List<MarkupReplacement>();
        int num = 0;

        foreach (object item in (IEnumerable)model)
        {
            string prefix2 = text + $"[{num}]";
            list.AddRange(collection: BuildModelReplacements(model: item, prefix: prefix2));
            num++;
        }

        string text2 = ((text.Length == 0) ? "Length" : (text + ".Length"));
        list.Add(item: new MarkupReplacement { Old = "[model[" + text2 + "]]", Value = num.ToString() });
        return list;
    }

    private IEnumerable<MarkupReplacement> BuildModelReplacementsForObject(object model, string prefix)
    {
        List<MarkupReplacement> replacements = [];

        IReadOnlyList<RuntimePropertyValue> properties = templateRenderService.GetPropertyValuesTemplateRenderFoundationOperation(
            templateRenderFoundationOperation: new TemplateRenderFoundationOperation
            {
                Value = model
            })
        .RuntimeProperties;

        foreach (RuntimePropertyValue property in properties)
        {
            object value = property.Value;
            string text = ((prefix.Length > 0) ? (prefix + "." + property.Name) : property.Name);

            if (property.IsValueType)
            {
                replacements.Add(item: new MarkupReplacement
                {
                    Old = "[model[" + prefix + "]]",
                    Value = model?.ToString() ?? string.Empty
                });

                replacements.Add(item: new MarkupReplacement
                {
                    Old = "[model[" + text + "]]",
                    Value = value?.ToString() ?? string.Empty
                });
            }
            else if (value != null)
            {
                replacements.AddRange(collection: BuildModelReplacements(model: value, prefix: text));
            }
        }

        RemoveInvalidReplacements(replacements: replacements);
        return replacements;
    }

    private IEnumerable<MarkupReplacement> BuildModelReplacementsForJObject(object model, string prefix)
    {
        IEnumerable<KeyValuePair<string, object>> source =
            GetJsonProperties(value: model);

        List<MarkupReplacement> replacements = [];

        foreach (KeyValuePair<string, object> token in source)
        {
            string text = ((prefix.Length > 0) ? (prefix + "." + token.Key) : token.Key);

            if (IsJsonValue(value: token.Value))
            {
                replacements.Add(item: new MarkupReplacement { Old = "[model[" + text + "]]", Value = token.Value.ToString() ?? string.Empty });
            }
            else if (token.Value != null)
            {
                replacements.AddRange(collection: BuildModelReplacements(model: token.Value, prefix: text));
            }

        }

        return replacements;
    }

    private IEnumerable<MarkupReplacement> BuildModelReplacementsForDynamicObject(object model, string prefix)
    {
        IDictionary<string, object> dynamicModel = (IDictionary<string, object>)model;

        List<MarkupReplacement> replacements = [];

        foreach (string key in dynamicModel.Keys)
        {
            string text = ((prefix.Length > 0) ? (prefix + "." + key) : key);
            replacements.Add(item: new MarkupReplacement { Old = "[model[" + text + "]]", Value = dynamicModel[key]?.ToString() ?? string.Empty });

            if (dynamicModel[key] != null && !dynamicModel[key].GetType()
                .IsValueType)
            {
                replacements.AddRange(collection: BuildModelReplacements(model: dynamicModel[key], prefix: text));
            }

        }

        return replacements;
    }

    private static Template ValidateTemplate(Template template, string parameterName)
    {
        if (template == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return template;
    }

    private static object ValidateModel(object model, string parameterName)
    {
        if (model == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return model;
    }

    private static RenderParams ValidateRenderParamsArgument(RenderParams renderParams, string parameterName)
    {
        if (renderParams == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return renderParams;
    }

    private static void ValidateAppId(int appId, string parameterName) =>
        ThrowIf(condition: appId < 1, message: parameterName + " must be greater than 0.");

    private static void ValidateTemplateName(string name, string parameterName) =>
        ThrowIf(condition: string.IsNullOrWhiteSpace(value: name), message: parameterName + " is required.");

    private static User ValidateUser(User user, string parameterName)
    {
        if (user == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return user;
    }

    private void RegexReplace(
        StringBuilder source,
        string matchExpression,
        Func<RegularExpressionMatch, string> action)
    {
        string result = ReplaceRegularExpression(
            input: source.ToString(),
            pattern: matchExpression,
            evaluator: (value, groups) => action(
                arg: new RegularExpressionMatch
                {
                    Value = value,
                    Groups = groups
                }));

        source.Clear();
        source.Append(value: result);
    }

    private void RegexMatch(
        StringBuilder source,
        string matchExpression,
        Action<RegularExpressionMatch> action) =>
        ForEachRegularExpressionMatch(
            input: source.ToString(),
            pattern: matchExpression,
            action: (value, groups) => action(
                obj: new RegularExpressionMatch
                {
                    Value = value,
                    Groups = groups
                }));

    private static string GetTagName(RegularExpressionMatch source) =>
        source.Value.Split(separator: '[')[2].Replace(oldValue: "]", newValue: "")
        .ToLowerInvariant();

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }

    private string ExecuteRenderTemplateRenderParamsConfig(
        Template template,
        object model,
        RenderParams renderParams,
        ContentManagementConfiguration config)
    {
        ValidateTemplate(template: template, parameterName: "template");
        ValidateModel(model: model, parameterName: "model");
        ValidateRenderParamsArgument(renderParams: renderParams, parameterName: "renderParams");

        List<MarkupReplacement> list = CopyToList(
            source: DefaultReplacements(renderParams: renderParams, config: config));

        list.Add(item: new MarkupReplacement { Old = "[model]", Value = Serialize(value: model) });
        list.AddRange(collection: BuildModelReplacements(model: model));

        if (IsLoggingEnabled(logLevel: LogLevel.Debug))
        {
            LogDebug(
                message: "Rendering template {Template} with {ReplacementCount} replacements.",
                args: [template.Name, list.Count]);
        }

        return ProcessContentString(key: template.ResourceKey, renderParams: renderParams, content: template.RawString, replacements: list);
    }

    private IReadOnlyCollection<App> GetApps() =>
        ExecuteFoundation(
            operation: new TemplateRenderFoundationOperation(),
            action: templateRenderService.GetAppsTemplateRenderFoundationOperation)
        .Apps;

    private IReadOnlyCollection<Component> GetComponents() =>
        ExecuteFoundation(
            operation: new TemplateRenderFoundationOperation(),
            action: templateRenderService.GetComponentsTemplateRenderFoundationOperation)
        .Components;

    private IReadOnlyCollection<Resource> GetResources() =>
        ExecuteFoundation(
            operation: new TemplateRenderFoundationOperation(),
            action: templateRenderService.GetResourcesTemplateRenderFoundationOperation)
        .Resources;

    private IReadOnlyCollection<Script> GetScripts() =>
        ExecuteFoundation(
            operation: new TemplateRenderFoundationOperation(),
            action: templateRenderService.GetScriptsTemplateRenderFoundationOperation)
        .Scripts;

    private IReadOnlyCollection<Template> GetTemplates() =>
        ExecuteFoundation(
            operation: new TemplateRenderFoundationOperation(),
            action: templateRenderService.GetTemplatesTemplateRenderFoundationOperation)
        .Templates;

    private Component GetComponent(string key) =>
        ExecuteFoundation(
            operation: new TemplateRenderFoundationOperation { Key = key },
            action: templateRenderService.GetComponentTemplateRenderFoundationOperation)
        .Component;

    private Script GetScript(string key) =>
        ExecuteFoundation(
            operation: new TemplateRenderFoundationOperation { Key = key },
            action: templateRenderService.GetScriptTemplateRenderFoundationOperation)
        .Script;

    private Resource GetResource(string key) =>
        ExecuteFoundation(
            operation: new TemplateRenderFoundationOperation { Key = key },
            action: templateRenderService.GetResourceTemplateRenderFoundationOperation)
        .Resource;

    private string GetMetadata(string key, string culture) =>
        ExecuteFoundation(
            operation: new TemplateRenderFoundationOperation
            {
                Key = key,
                Culture = culture
            },
            action: templateRenderService.GetMetadataTemplateRenderFoundationOperation)
        .Content;

    private string Serialize(object value) =>
        ExecuteFoundation(
            operation: new TemplateRenderFoundationOperation { Value = value },
            action: templateRenderService.SerializeTemplateRenderFoundationOperation)
        .Content;

    private string SerializeIgnoringReferences(object value) =>
        ExecuteFoundation(
            operation: new TemplateRenderFoundationOperation { Value = value },
            action: templateRenderService.SerializeIgnoringReferencesTemplateRenderFoundationOperation)
        .Content;

    private object ParseJson(string json) =>
        ExecuteFoundation(
            operation: new TemplateRenderFoundationOperation { Content = json },
            action: templateRenderService.ParseJsonTemplateRenderFoundationOperation)
        .Value;

    private object NormalizeJson(object value) =>
        ExecuteFoundation(
            operation: new TemplateRenderFoundationOperation { Value = value },
            action: templateRenderService.NormalizeJsonTemplateRenderFoundationOperation)
        .Value;

    private bool IsJsonObject(object value) =>
        ExecuteFoundation(
            operation: new TemplateRenderFoundationOperation { Value = value },
            action: templateRenderService.IsJsonObjectTemplateRenderFoundationOperation)
        .Condition;

    private bool IsJsonArray(object value) =>
        ExecuteFoundation(
            operation: new TemplateRenderFoundationOperation { Value = value },
            action: templateRenderService.IsJsonArrayTemplateRenderFoundationOperation)
        .Condition;

    private bool IsJsonValue(object value) =>
        ExecuteFoundation(
            operation: new TemplateRenderFoundationOperation { Value = value },
            action: templateRenderService.IsJsonValueTemplateRenderFoundationOperation)
        .Condition;

    private IReadOnlyCollection<KeyValuePair<string, object>> GetJsonProperties(
        object value) =>
        ExecuteFoundation(
            operation: new TemplateRenderFoundationOperation { Value = value },
            action: templateRenderService.GetJsonPropertiesTemplateRenderFoundationOperation)
        .JsonProperties;

    private string ExecuteWorkflow(string baseAddress, string content) =>
        ExecuteFoundation(
            operation: new TemplateRenderFoundationOperation
            {
                BaseAddress = baseAddress,
                Content = content
            },
            action: templateRenderService.ExecuteWorkflowTemplateRenderFoundationOperation)
        .Content;

    private bool IsLoggingEnabled(LogLevel logLevel) =>
        ExecuteFoundation(
            operation: new TemplateRenderFoundationOperation { LogLevel = logLevel },
            action: templateRenderService.IsLoggingEnabledTemplateRenderFoundationOperation)
        .Condition;

    private void LogDebug(string message, params object[] args) =>
        ExecuteFoundation(
            operation: new TemplateRenderFoundationOperation
            {
                Message = message,
                Arguments = args
            },
            action: templateRenderService.LogDebugTemplateRenderFoundationOperation);

    private string ReplaceRegularExpression(
        string input,
        string pattern,
        Func<string, IReadOnlyDictionary<string, string>, string> evaluator) =>
        ExecuteFoundation(
            operation: new TemplateRenderFoundationOperation
            {
                Input = input,
                Pattern = pattern,
                Evaluator = evaluator
            },
            action: templateRenderService.ReplaceRegularExpressionTemplateRenderFoundationOperation)
        .Content;

    private void ForEachRegularExpressionMatch(
        string input,
        string pattern,
        Action<string, IReadOnlyDictionary<string, string>> action) =>
        ExecuteFoundation(
            operation: new TemplateRenderFoundationOperation
            {
                Input = input,
                Pattern = pattern,
                MatchAction = action
            },
            action: templateRenderService.ForEachRegularExpressionMatchTemplateRenderFoundationOperation);

    private static TemplateRenderFoundationOperation ExecuteFoundation(
        TemplateRenderFoundationOperation operation,
        Func<TemplateRenderFoundationOperation, TemplateRenderFoundationOperation> action) =>
        action(arg: operation);

    private static T FindFirst<T>(IEnumerable<T> source, Func<T, bool> predicate)
        where T : class
    {
        foreach (T item in source ?? [])
        {
            if (predicate(arg: item))
            {
                return item;
            }
        }

        return null;
    }

    private static T FindRequired<T>(IEnumerable<T> source, Func<T, bool> predicate)
        where T : class =>
        FindFirst(source: source, predicate: predicate)
        ?? throw new InvalidOperationException(message: "Sequence contains no matching element.");

    private static List<T> CopyToList<T>(IEnumerable<T> source)
    {
        List<T> items = [];

        foreach (T item in source ?? [])
        {
            items.Add(item: item);
        }

        return items;
    }

    private static bool TryGetFirstValue(
        IDictionary<string, object> source,
        out object value)
    {
        if (source == null)
        {
            value = null;
            return false;
        }

        foreach (KeyValuePair<string, object> item in source)
        {
            value = item.Value;
            return true;
        }

        value = null;
        return false;
    }

    private static void RemoveInvalidReplacements(List<MarkupReplacement> replacements)
    {
        for (int index = replacements.Count - 1; index >= 0; index--)
        {
            if (replacements[index].Old == null || replacements[index].New == null)
            {
                replacements.RemoveAt(index: index);
            }
        }
    }
}