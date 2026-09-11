// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using cCoder.ContentManagement.Services.Foundations.Rendering;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Models.PageRendering;
using cCoder.ContentManagement.Models.RegularExpressions;
using cCoder.ContentManagement.Models.Rendering;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class ComponentRenderProcessingService(
    IComponentRenderService componentRenderService,
    ContentManagementConfiguration config)
        : IComponentRenderProcessingService
{
    private const string TagPattern = "\\[TYPE\\[[A-Za-z\\d_/-]*\\][A-Za-z\\d_/-]*\\=*\\\"*-*[A-Za-z\\d_/-]*\\\"*\\]";

    public string RenderComponentRenderOperation(
        ComponentRenderOperation componentRenderOperation) =>
        TryCatch<string>(operation: () =>
    {
        ValidateRenderComponentOperation(inputs: [componentRenderOperation]);

        componentRenderOperation.Result = componentRenderOperation.Component != null
            ? RenderComponentComponentRenderParams(
                component: componentRenderOperation.Component,
                renderParams: componentRenderOperation.RenderParams)
            : RenderUser(
                appId: componentRenderOperation.AppId,
                name: componentRenderOperation.Name,
                user: componentRenderOperation.User,
                culture: componentRenderOperation.Culture,
                theme: componentRenderOperation.Theme);

        return componentRenderOperation.Result;
    });

    internal string RenderUser(int appId, string name, User user, string culture, string theme) =>
        TryCatch<string>(operation: () =>
    {
        ValidateRenderUser(inputs: [appId, name, user, culture, theme]);
        ValidateAppId(appId: appId, parameterName: "appId");
        ValidateName(name: name, parameterName: "name");
        ValidateTheme(theme: theme, parameterName: "theme");
        ValidateUser(user: user, parameterName: "user");
        culture ??= user.DefaultCultureId;

        App app = GetApps()
            .Where(predicate: existingApp => existingApp.Id == appId)
            .Select(selector: existingApp => new App
            {
                Id = existingApp.Id,
                DefaultCultureId = existingApp.DefaultCultureId,
                TenantId = existingApp.TenantId,
                Name = existingApp.Name,
                Domain = existingApp.Domain,
                DefaultTheme = existingApp.DefaultTheme,
                ConfigJson = existingApp.ConfigJson
            })
            .FirstOrDefault();

        if (app != null)
        {
            app.Components = GetComponents()
                .Where(predicate: existingComponent => existingComponent.AppId == appId)
                .ToArray();

            app.Resources = GetResources()
                .Where(predicate: existingResource => existingResource.AppId == appId)
                .ToArray();

            app.Scripts = GetScripts()
                .Where(predicate: existingScript => existingScript.AppId == appId)
                .ToArray();
        }

        Component component = app?.Components
            .Where(predicate: existingComponent => existingComponent.AppId == appId)
            .FirstOrDefault(predicate: existingComponent =>
                existingComponent.Name.Equals(value: name, comparisonType: StringComparison.OrdinalIgnoreCase))
            ?? GetComponent(key: "component|" + name.ToLower())
            ?? throw new InvalidOperationException(message: "Component '" + name + "' was not found.");

        ComponentRenderParams renderParams = new()
        {
            Theme = theme ?? "Default",
            App = app,
            User = user,
            Culture = culture
        };

        return ExecuteRenderComponentComponentRenderParams(component: component, renderParams: renderParams);

    });

    internal string RenderComponentComponentRenderParams(Component component, ComponentRenderParams renderParams) =>
        TryCatch<string>(operation: () =>
    {
        ValidateRenderComponentComponentRenderParams(inputs: [component, renderParams]);
        ValidateComponent(component: component, parameterName: "component");
        ValidateComponentRenderParams(renderParams: renderParams, parameterName: "renderParams");
        ICollection<MarkupReplacement> replacements = DefaultReplacements(renderParams: renderParams);
        return $"<section name='{component.Name}' class='component' data-id='{component.Id}' data-resource-key='{component.ResourceKey}'>{ProcessContentString(key: component.ResourceKey, renderParams: renderParams, content: component.Content, replacements: replacements)}<script type='text/javascript' nonce='{ContentSecurityPolicyNonceContract.Placeholder}'>{ProcessContentString(key: component.ResourceKey, renderParams: renderParams, content: component.Script, replacements: replacements)}</script></section>";

    });

    private ICollection<MarkupReplacement> DefaultReplacements(RenderParams renderParams)
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

        int num = 10;
        List<MarkupReplacement> list = new List<MarkupReplacement>(capacity: num);
        CollectionsMarshal.SetCount(list: list, count: num);
        Span<MarkupReplacement> span = CollectionsMarshal.AsSpan(list: list);

        span[0] = new MarkupReplacement { Old = "[[user]]", Value = Serialize(value: new
        {
            Id = renderParams.User?.Id,
            DefaultCultureId = renderParams.User?.DefaultCultureId,
            DisplayName = renderParams.User?.DisplayName,
            Email = renderParams.User?.Email
        }) };

        span[1] = new MarkupReplacement { Old = "[[displayname]]", Value = renderParams.User?.DisplayName };
        span[2] = new MarkupReplacement { Old = "[[loginlink]]", Value = (renderParams.User?.Id == "Guest") ? "<a href='/Login'>[resource_displayname[Login]]</a>" : "<a name='logout' href=''>[resource_displayname[Logout]]</a>" };
        span[3] = new MarkupReplacement { Old = "[[date]]", Value = DateTimeOffset.UtcNow.ToString(format: "dd MMM yyyy") };
        span[4] = new MarkupReplacement { Old = "[[culture]]", Value = text2 };

        span[5] = new MarkupReplacement { Old = "[[lang]]", Value = text2.Split(separator: '-')
            .First() };

        span[6] = new MarkupReplacement { Old = "[app[name]]", Value = renderParams.App?.Name };
        span[7] = new MarkupReplacement { Old = "[app[domain]]", Value = renderParams.App?.Domain };
        span[8] = new MarkupReplacement { Old = "[app[root]]", Value = "https://" + renderParams.App?.Domain + text3 + "/" };
        span[9] = new MarkupReplacement { Old = "[app[id]]", Value = renderParams.App?.Id.ToString() };
        List<MarkupReplacement> list2 = list;

        if (config != null)
        {
            if (!string.IsNullOrWhiteSpace(value: config.WorkflowServiceUrl))
            {
                list2.Add(item: new MarkupReplacement { Old = "[api[workflow]]", Value = config.WorkflowServiceUrl });
            }

            list2.Add(item: new MarkupReplacement { Old = "[api[root]]", Value = "https://" + renderParams.App?.Domain + text3 + "/Api/" });
        }

        if (renderParams is ComponentRenderParams componentRenderParams)
        {
            list2.Add(item: new MarkupReplacement { Old = "[theme[name]]", Value = componentRenderParams.Theme });
            IDictionary<string, object> dictionary = default(IDictionary<string, object>);
            object value3 = null;

            if ((TryGetThemeDictionary(config: renderParams.App.Config, themeDictionary: out dictionary)) && dictionary.TryGetValue(key: componentRenderParams.Theme, value: out value3))
            {
                list2.AddRange(collection: BuildThemeReplacements(model: value3));
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

        if (renderParams is ComponentRenderParams renderParams2)
        {
            Dms(key: key, source: result, renderParams: renderParams2, replacements: replacements);
        }

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

        if ((replacements != null || !(renderParams is ComponentRenderParams)) && replacements == null)
        {
            throw new ValidationException(message: "replacements is required.");
        }
    }

    private static (string type, string name, string[] options) SplitMatch(
        RegularExpressionMatch match)
    {
        string[] array = match.Value
            .Split(separator: "[");

        string[] array2 = array.Last()
            .Split(separator: "]");

        return (type: array[1].ToLower(), name: array2[0].ToLower(), options: array2[1].Split(separator: "|", options: StringSplitOptions.RemoveEmptyEntries));
    }

    private void Component(string key, RenderParams renderParams, IEnumerable<MarkupReplacement> replacements, StringBuilder result) =>
        RegexReplace(source: result, matchExpression: "\\[TYPE\\[[A-Za-z\\d_/-]*\\][A-Za-z\\d_/-]*\\=*\\\"*-*[A-Za-z\\d_/-]*\\\"*\\]".Replace(oldValue: "TYPE", newValue: "component"), action: match =>
                                                                                                                                  {
                                                                                                                                      (string _, string name, string[] options) tag = SplitMatch(match: match);
                                                                                                                                      Component component = renderParams.App?.Components?.FirstOrDefault(predicate: (Component c) => c.Name.Equals(value: tag.name, comparisonType: StringComparison.CurrentCultureIgnoreCase)) ?? GetComponent(key: "component|" + tag.name);
                                                                                                                                      return (component == null) ? ("[[Missing Component:" + tag.name + "]]") : ProcessContentString(key: key, renderParams: renderParams, content: BuildComponentMarkup(component: component, tag: tag, replacements: replacements, renderParams: renderParams), replacements: replacements);
                                                                                                                                  });

    private string BuildComponentMarkup(Component component, (string type, string name, string[] options) tag, IEnumerable<MarkupReplacement> replacements, RenderParams renderParams)
    {
        string value = string.Join(separator: " ", values: tag.options
            .Where(predicate: option => option.StartsWith(value: "class="))
            .Select(selector: option => option.Replace(oldValue: "class=", newValue: "")));

        string content = $"<section name='{component.Name}' class='component {value}' data-id='{component.Id}' data-resource-key='{component.ResourceKey}' {string.Join(separator: " ", values: tag.options.Where(predicate: (string option) => !option.StartsWith(value: "class=")))}>\r\n                        {ProcessContentString(key: component.ResourceKey, renderParams: renderParams, content: component.Content, replacements: replacements)}\r\n                        <script type='text/javascript' nonce='{ContentSecurityPolicyNonceContract.Placeholder}'>{ProcessContentString(key: component.ResourceKey, renderParams: renderParams, content: component.Script, replacements: replacements)}</script>\r\n                    </section>";
        return ProcessContentString(key: component.ResourceKey, renderParams: renderParams, content: content, replacements: replacements);
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
                                                                                                                                       Script obj = renderParams.App?.Scripts?.FirstOrDefault(predicate: (Script s) => s.Name.Equals(value: name, comparisonType: StringComparison.CurrentCultureIgnoreCase));
                                                                                                                                       return ProcessContentString(key: key, renderParams: renderParams, content: obj?.Content ?? script.Content, replacements: replacements);
                                                                                                                                   }

                                                                                                                                   return string.Empty;
                                                                                                                               });

    private void ExecuteAsync(string key, StringBuilder source, RenderParams renderParams, IEnumerable<MarkupReplacement> replacements) =>
        RegexReplace(source: source, matchExpression: "\\[execute\\](.*?)\\[/execute\\]", action: match =>
                                                                                                                                     {
                                                                                                                                         string value = match.Groups["1"];
                                                                                                                                         string json = replacements.FirstOrDefault(predicate: (MarkupReplacement r) => r.Old == "[model]")?.New ?? "{}";

                                                                                                                                         string content = SerializeIgnoringReferences(value: new
                                                                                                                                         {
                                                                                                                                             Script = value,
                                                                                                                                             Model = ParseJson(json: json)
                                                                                                                                         });

                                                                                                                                         string result = ExecuteWorkflow(
                                                                                                                                             baseAddress: replacements.First(predicate: replacement => replacement.Old == "[api[workflow]]").New,
                                                                                                                                             content: content);

                                                                                                                                         return ProcessContentString(key: key, renderParams: renderParams, content: result, replacements: replacements);
                                                                                                                                     });

    private void Dms(string key, StringBuilder source, ComponentRenderParams renderParams, IEnumerable<MarkupReplacement> replacements) =>
        RegexReplace(source: source, matchExpression: "\\[dms\\[[A-Za-z\\d_/. \\-]*\\]\\]", action: match =>
                                                                                                                                     {
                                                                                                                                         string path = match.Value.Replace(oldValue: "[dms[", newValue: "")
                                                                                                                                             .Replace(oldValue: "]]", newValue: "")
                                                                                                                                             .ToLowerInvariant();

                                                                                                                                         string latestTextContent = GetLatestTextContent(
                                                                                                                                             appId: renderParams.App.Id,
                                                                                                                                             path: path);

                                                                                                                                         return string.IsNullOrEmpty(value: latestTextContent) ? string.Empty : ProcessContentString(key: key, renderParams: renderParams, content: latestTextContent, replacements: replacements);
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
        List<Resource> list2 = new List<Resource>();
        list2.AddRange(collection: ExecuteSectionForCultureResource(potentials: renderParams.App.Resources, key: key, culture: renderParams.Culture ?? string.Empty));
        list.AddRange(collection: list2);
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

        RegexReplace(source: source, matchExpression: "\\[TYPE\\[[A-Za-z\\d_/-]*\\][A-Za-z\\d_/-]*\\=*\\\"*-*[A-Za-z\\d_/-]*\\\"*\\]".Replace(oldValue: "TYPE", newValue: "resource_displayname"), action: match => ProcessContentString(key: key, renderParams: renderParams, content: known.FirstOrDefault(predicate: resource => resource.Name.Equals(value: GetTagName(source: match), comparisonType: StringComparison.CurrentCultureIgnoreCase))?.DisplayName ?? GetTagName(source: match)
            .ToLower(), replacements: replacements));

        RegexReplace(source: source, matchExpression: "\\[TYPE\\[[A-Za-z\\d_/-]*\\][A-Za-z\\d_/-]*\\=*\\\"*-*[A-Za-z\\d_/-]*\\\"*\\]".Replace(oldValue: "TYPE", newValue: "resource_shortdisplayname"), action: match => ProcessContentString(key: key, renderParams: renderParams, content: known.FirstOrDefault(predicate: resource => resource.Name.Equals(value: GetTagName(source: match), comparisonType: StringComparison.CurrentCultureIgnoreCase))?.ShortDisplayName ?? GetTagName(source: match)
            .ToLower(), replacements: replacements));

        RegexReplace(source: source, matchExpression: "\\[TYPE\\[[A-Za-z\\d_/-]*\\][A-Za-z\\d_/-]*\\=*\\\"*-*[A-Za-z\\d_/-]*\\\"*\\]".Replace(oldValue: "TYPE", newValue: "resource_description"), action: match => ProcessContentString(key: key, renderParams: renderParams, content: known.FirstOrDefault(predicate: resource => resource.Name.Equals(value: GetTagName(source: match), comparisonType: StringComparison.CurrentCultureIgnoreCase))?.Description ?? GetTagName(source: match)
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

    private IEnumerable<MarkupReplacement> BuildIEnumerableThemeReplacements<T>(T model, string prefix) =>
        model.GetType()
        .GetProperties()
        .SelectMany(selector: property =>
            {
                object value = property.GetValue(obj: model);
                string text = ((prefix.Length > 0) ? (prefix + "." + property.Name) : property.Name);

                if (property.PropertyType.IsValueType || property.PropertyType == typeof(string))
                {
                    MarkupReplacement[] array = new MarkupReplacement[2];
                    string old = "[theme[" + prefix + "]]";
                    object obj = model?.ToString();

                    if (obj == null)
                    {
                        obj = string.Empty;
                    }

                    array[0] = new MarkupReplacement { Old = old, Value = (string)obj };
                    array[1] = new MarkupReplacement { Old = "[theme[" + text + "]]", Value = value?.ToString() ?? string.Empty };
                    return array;
                }

                IEnumerable<MarkupReplacement> result;

                if (value == null)
                {
                    IEnumerable<MarkupReplacement> enumerable = Array.Empty<MarkupReplacement>();
                    result = enumerable;
                }
                else
                {
                    result = BuildThemeReplacements(model: value, prefix: text);
                }

                return result;
            })
        .Where(predicate: replacement => replacement.Old != null && replacement.New != null);

    private IEnumerable<MarkupReplacement> BuildJObjectThemeReplacements<T>(T model, string prefix)
    {
        IEnumerable<KeyValuePair<string, object>> source =
            GetJsonProperties(value: model);

        return source.SelectMany(selector: token =>
        {
            string text = ((prefix.Length > 0) ? (prefix + "." + token.Key) : token.Key);

            if (IsJsonValue(value: token.Value))
            {
                return new[] { new MarkupReplacement { Old = "[theme[" + text + "]]", Value = token.Value.ToString() ?? string.Empty } };
            }

            IEnumerable<MarkupReplacement> result;

            if (token.Value == null)
            {
                IEnumerable<MarkupReplacement> enumerable = Array.Empty<MarkupReplacement>();
                result = enumerable;
            }
            else
            {
                result = BuildThemeReplacements(model: token.Value, prefix: text);
            }

            return result;
        });
    }

    private IEnumerable<MarkupReplacement> BuildDynamicThemeReplacements<T>(T model, string prefix)
    {
        IDictionary<string, object> dynamicModel = (IDictionary<string, object>)(object)model;

        return dynamicModel.Keys.SelectMany(selector: key =>
        {
            string text = ((prefix.Length > 0) ? (prefix + "." + key) : key);
            int num = 1;
            List<MarkupReplacement> list = new List<MarkupReplacement>(capacity: num);
            CollectionsMarshal.SetCount(list: list, count: num);
            CollectionsMarshal.AsSpan(list: list)[0] = new MarkupReplacement { Old = "[theme[" + text + "]]", Value = dynamicModel[key]?.ToString() ?? string.Empty };
            List<MarkupReplacement> list2 = list;

            if (dynamicModel[key] != null && !dynamicModel[key].GetType()
                .IsValueType)
            {
                list2.AddRange(collection: BuildThemeReplacements(model: dynamicModel[key], prefix: text));
            }

            return list2;
        });
    }

    private static Component ValidateComponent(Component component, string parameterName)
    {
        if (component == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return component;
    }

    private static ComponentRenderParams ValidateComponentRenderParams(ComponentRenderParams renderParams, string parameterName)
    {
        if (renderParams == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return renderParams;
    }

    private static void ValidateAppId(int appId, string parameterName) =>
        ThrowIf(condition: appId < 1, message: parameterName + " must be greater than 0.");

    private static void ValidateName(string name, string parameterName) =>
        ThrowIf(condition: string.IsNullOrWhiteSpace(value: name), message: parameterName + " is required.");

    private static void ValidateTheme(string theme, string parameterName) =>
        ThrowIf(condition: string.IsNullOrWhiteSpace(value: theme), message: parameterName + " is required.");

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

    private static Resource ExecuteGetClosestCulturalMatchResource(IEnumerable<Resource> potentials, string culture)
    {
        Resource resource = null;

        List<string> list = (culture ?? string.Empty).ToLowerInvariant()
            .Split(separator: '-')
            .ToList();

        int num = list.Count;
        string resultCulture = string.Empty;

        while (resource == null && resultCulture != null)
        {
            resultCulture = string.Join(separator: "-", values: list.Take(count: num));
            resource = potentials.FirstOrDefault(predicate: (Resource resource2) => string.Equals(a: resource2.Culture, b: resultCulture, comparisonType: StringComparison.OrdinalIgnoreCase));
            num--;

            if (num == 0)
            {
                resultCulture = null;
            }
        }

        return resource ?? potentials.FirstOrDefault(predicate: (Resource resource2) => string.IsNullOrEmpty(value: resource2.Culture));
    }

    private IReadOnlyCollection<App> GetApps() =>
        componentRenderService.GetAppsComponentRenderFoundationOperation(
            componentRenderFoundationOperation: new ComponentRenderFoundationOperation())
        .Apps;

    private IReadOnlyCollection<Component> GetComponents() =>
        componentRenderService.GetComponentsComponentRenderFoundationOperation(
            componentRenderFoundationOperation: new ComponentRenderFoundationOperation())
        .Components;

    private IReadOnlyCollection<Resource> GetResources() =>
        componentRenderService.GetResourcesComponentRenderFoundationOperation(
            componentRenderFoundationOperation: new ComponentRenderFoundationOperation())
        .Resources;

    private IReadOnlyCollection<Script> GetScripts() =>
        componentRenderService.GetScriptsComponentRenderFoundationOperation(
            componentRenderFoundationOperation: new ComponentRenderFoundationOperation())
        .Scripts;

    private Component GetComponent(string key) =>
        componentRenderService.GetComponentComponentRenderFoundationOperation(
            componentRenderFoundationOperation: new ComponentRenderFoundationOperation
            {
                Key = key
            })
        .Component;

    private Script GetScript(string key) =>
        componentRenderService.GetScriptComponentRenderFoundationOperation(
            componentRenderFoundationOperation: new ComponentRenderFoundationOperation
            {
                Key = key
            })
        .Script;

    private Resource GetResource(string key) =>
        componentRenderService.GetResourceComponentRenderFoundationOperation(
            componentRenderFoundationOperation: new ComponentRenderFoundationOperation
            {
                Key = key
            })
        .Resource;

    private string GetMetadata(string key, string culture) =>
        componentRenderService.GetMetadataComponentRenderFoundationOperation(
            componentRenderFoundationOperation: new ComponentRenderFoundationOperation
            {
                Key = key,
                Culture = culture
            })
        .Content;

    private string GetLatestTextContent(int appId, string path) =>
        componentRenderService.GetLatestTextContentComponentRenderFoundationOperation(
            componentRenderFoundationOperation: new ComponentRenderFoundationOperation
            {
                AppId = appId,
                Path = path
            })
        .Content;

    private string Serialize(object value) =>
        componentRenderService.SerializeComponentRenderFoundationOperation(
            componentRenderFoundationOperation: new ComponentRenderFoundationOperation
            {
                Value = value
            })
        .Content;

    private string SerializeIgnoringReferences(object value) =>
        componentRenderService.SerializeIgnoringReferencesComponentRenderFoundationOperation(
            componentRenderFoundationOperation: new ComponentRenderFoundationOperation
            {
                Value = value
            })
        .Content;

    private object ParseJson(string json) =>
        componentRenderService.ParseJsonComponentRenderFoundationOperation(
            componentRenderFoundationOperation: new ComponentRenderFoundationOperation
            {
                Content = json
            })
        .Value;

    private bool IsJsonObject(object value) =>
        componentRenderService.IsJsonObjectComponentRenderFoundationOperation(
            componentRenderFoundationOperation: new ComponentRenderFoundationOperation
            {
                Value = value
            })
        .Condition;

    private bool IsJsonValue(object value) =>
        componentRenderService.IsJsonValueComponentRenderFoundationOperation(
            componentRenderFoundationOperation: new ComponentRenderFoundationOperation
            {
                Value = value
            })
        .Condition;

    private IReadOnlyCollection<KeyValuePair<string, object>> GetJsonProperties(
        object value) =>
        componentRenderService.GetJsonPropertiesComponentRenderFoundationOperation(
            componentRenderFoundationOperation: new ComponentRenderFoundationOperation
            {
                Value = value
            })
        .JsonProperties;

    private string ExecuteWorkflow(string baseAddress, string content) =>
        componentRenderService.ExecuteWorkflowComponentRenderFoundationOperation(
            componentRenderFoundationOperation: new ComponentRenderFoundationOperation
            {
                BaseAddress = baseAddress,
                Content = content
            })
        .Content;

    private string ReplaceRegularExpression(
        string input,
        string pattern,
        Func<string, IReadOnlyDictionary<string, string>, string> evaluator) =>
        componentRenderService.ReplaceRegularExpressionComponentRenderFoundationOperation(
            componentRenderFoundationOperation: new ComponentRenderFoundationOperation
            {
                Input = input,
                Pattern = pattern,
                Evaluator = evaluator
            })
        .Content;

    private void ForEachRegularExpressionMatch(
        string input,
        string pattern,
        Action<string, IReadOnlyDictionary<string, string>> action) =>
        componentRenderService.ForEachRegularExpressionMatchComponentRenderFoundationOperation(
            componentRenderFoundationOperation: new ComponentRenderFoundationOperation
            {
                Input = input,
                Pattern = pattern,
                MatchAction = action
            });

    private string ExecuteRenderComponentComponentRenderParams(Component component, ComponentRenderParams renderParams)
    {
        ValidateComponent(component: component, parameterName: "component");
        ValidateComponentRenderParams(renderParams: renderParams, parameterName: "renderParams");
        ICollection<MarkupReplacement> replacements = DefaultReplacements(renderParams: renderParams);
        return $"<section name='{component.Name}' class='component' data-id='{component.Id}' data-resource-key='{component.ResourceKey}'>{ProcessContentString(key: component.ResourceKey, renderParams: renderParams, content: component.Content, replacements: replacements)}<script type='text/javascript' nonce='{ContentSecurityPolicyNonceContract.Placeholder}'>{ProcessContentString(key: component.ResourceKey, renderParams: renderParams, content: component.Script, replacements: replacements)}</script></section>";
    }

    private static IEnumerable<Resource> ExecuteSectionForCultureResource(IEnumerable<Resource> potentials, string key, string culture)
    {
        List<Resource> list = new List<Resource>();

        foreach (IGrouping<string, Resource> item in potentials
            .Where(predicate: resource => string.Equals(a: resource.Key, b: key, comparisonType: StringComparison.OrdinalIgnoreCase))
            .GroupBy(keySelector: resource => resource.Name.ToLowerInvariant()))
        {
            Resource closestCulturalMatch = ExecuteGetClosestCulturalMatchResource(potentials: item, culture: culture);

            if (closestCulturalMatch != null)
            {
                list.Add(item: closestCulturalMatch);
            }
        }

        return list;
    }
}