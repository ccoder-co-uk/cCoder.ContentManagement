// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Rendering.Brokers;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.ContentManagement.Services.Foundations.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using cCoder.ContentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class TemplateRenderProcessingService(
    IMetadataReaderBroker metadataCache,
    ICommonObjectReaderBroker objectCache,
    IJsonBroker jsonBroker,
    ITemplateRenderService templateRenderService,
    IWorkflowExecutionBroker workflowExecutionBroker,
    ContentManagementConfiguration config = null,
    ILogger<TemplateRenderProcessingService> log = null)
        : ITemplateRenderProcessingService
{
    private const string TagPattern = "\\[TYPE\\[[A-Za-z\\d_/-]*\\][A-Za-z\\d_/-]*\\=*\\\"*-*[A-Za-z\\d_/-]*\\\"*\\]";

    public string RenderTemplateRenderOperation(
        TemplateRenderOperation operation) =>
        TryCatch<string>(operation: () =>
    {
        ValidateRenderTemplateOperation(inputs: [operation]);

        operation.Result = operation.Template != null
            ? RenderTemplateRenderParams(
                template: operation.Template,
                model: operation.Model,
                renderParams: operation.RenderParams)
            : RenderUser(
                appId: operation.AppId,
                name: operation.Name,
                model: operation.Model,
                user: operation.User,
                culture: operation.Culture);

        return operation.Result;
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

        App app = templateRenderService
            .Execute<IAppService, IQueryable<App>>(
                name: "AppStorage",
                operation: service => service.GetAllApp(
                    ignoreFilters: true))
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

        if (app == null)
        {
            throw new InvalidOperationException(message: $"App '{appId}' was not found.");
        }

        app.Components = templateRenderService
            .Execute<IComponentService, IQueryable<Component>>(
                name: "ComponentStorage",
                operation: service => service.GetAllComponent(
                    ignoreFilters: true))
            .Where(predicate: existingComponent => existingComponent.AppId == appId)
            .ToArray();

        app.Resources = templateRenderService
            .Execute<IResourceService, IQueryable<Resource>>(
                name: "ResourceStorage",
                operation: service => service.GetAllResource(
                    ignoreFilters: true))
            .Where(predicate: existingResource => existingResource.AppId == appId)
            .ToArray();

        app.Scripts = templateRenderService
            .Execute<IScriptService, IQueryable<Script>>(
                name: "ScriptStorage",
                operation: service => service.GetAllScript(
                    ignoreFilters: true))
            .Where(predicate: existingScript => existingScript.AppId == appId)
            .ToArray();

        Template template = templateRenderService
            .Execute<ITemplateService, IQueryable<Template>>(
                name: "TemplateStorage",
                operation: service => service.GetAllTemplate(
                    ignoreFilters: true))
            .Where(predicate: existingTemplate => existingTemplate.AppId == appId)
            .ToArray()
            .FirstOrDefault(predicate: existingTemplate =>
                existingTemplate.Name.Equals(value: name, comparisonType: StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException(message: "Template '" + name + "' was not found.");

        TemplateRenderParams templateRenderParams = new()
        {
            App = app,
            User = user,
            Culture = culture
        };

        return ExecuteRenderTemplateRenderParamsConfig(template: template, model: model, renderParams: templateRenderParams, config: config, log: log);

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

        List<ReplacementDependency> list = DefaultReplacements(renderParams: renderParams, config: config)
            .ToList();

        list.Add(item: new ReplacementDependency(old: "[model]", @new: jsonBroker.Serialize(value: model)));
        list.AddRange(collection: BuildModelReplacements(model: model));

        if (log != null && log.IsEnabled(logLevel: LogLevel.Debug))
        {
            log.LogDebug(
                message: "Rendering template {Template} with {ReplacementCount} replacements.",
                args: [template.Name, list.Count]);
        }

        return ProcessContentString(key: template.ResourceKey, renderParams: renderParams, content: template.RawString, replacements: list);

    });

    private ICollection<ReplacementDependency> DefaultReplacements(
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

        int num = 10;
        List<ReplacementDependency> list = new List<ReplacementDependency>(capacity: num);
        CollectionsMarshal.SetCount(list: list, count: num);
        Span<ReplacementDependency> span = CollectionsMarshal.AsSpan(list: list);

        span[0] = new ReplacementDependency(old: "[[user]]", @new: jsonBroker.Serialize(value: new
        {
            Id = renderParams.User?.Id,
            DefaultCultureId = renderParams.User?.DefaultCultureId,
            DisplayName = renderParams.User?.DisplayName,
            Email = renderParams.User?.Email
        }));

        span[1] = new ReplacementDependency(old: "[[displayname]]", @new: renderParams.User?.DisplayName);
        span[2] = new ReplacementDependency(old: "[[loginlink]]", @new: (renderParams.User?.Id == "Guest") ? "<a href='/Login'>[resource_displayname[Login]]</a>" : "<a name='logout' href=''>[resource_displayname[Logout]]</a>");
        span[3] = new ReplacementDependency(old: "[[date]]", @new: DateTimeOffset.UtcNow.ToString(format: "dd MMM yyyy"));
        span[4] = new ReplacementDependency(old: "[[culture]]", @new: text2);

        span[5] = new ReplacementDependency(old: "[[lang]]", @new: text2.Split(separator: '-')
            .First());

        span[6] = new ReplacementDependency(old: "[app[name]]", @new: renderParams.App?.Name);
        span[7] = new ReplacementDependency(old: "[app[domain]]", @new: renderParams.App?.Domain);
        span[8] = new ReplacementDependency(old: "[app[root]]", @new: "https://" + renderParams.App?.Domain + text3 + "/");
        span[9] = new ReplacementDependency(old: "[app[id]]", @new: renderParams.App?.Id.ToString());
        List<ReplacementDependency> list2 = list;

        if (config != null)
        {
            if (!string.IsNullOrWhiteSpace(value: config.WorkflowServiceUrl))
            {
                list2.Add(item: new ReplacementDependency(
                    old: "[api[workflow]]",
                    @new: config.WorkflowServiceUrl));
            }

            list2.Add(item: new ReplacementDependency(old: "[api[root]]", @new: "https://" + renderParams.App?.Domain + text3 + "/Api/"));
        }

        if (renderParams is TemplateRenderParams)
        {
            list2.Add(item: new ReplacementDependency(old: "[theme[name]]", @new: "Default"));
            IDictionary<string, object> source = default(IDictionary<string, object>);

            if (TryGetThemeDictionary(config: renderParams.App.Config, themeDictionary: out source) && source.Any())
            {
                list2.AddRange(collection: BuildThemeReplacements(model: source.First()
                    .Value));
            }
        }

        return list2;
    }

    private string ProcessContentString(string key, RenderParams renderParams, string content, IEnumerable<ReplacementDependency> replacements)
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

        foreach (ReplacementDependency replacement in replacements)
        {
            result.Replace(oldValue: replacement.Old, newValue: replacement.New);
        }

        return result.ToString();
    }

    private static void ValidateRenderParams(RenderParams renderParams, IEnumerable<ReplacementDependency> replacements)
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

    private static (string type, string name, string[] options) SplitMatch(Match match)
    {
        string[] array = match.ToString()
            .Split(separator: "[");

        string[] array2 = array.Last()
            .Split(separator: "]");

        return (type: array[1].ToLower(), name: array2[0].ToLower(), options: array2[1].Split(separator: "|", options: StringSplitOptions.RemoveEmptyEntries));
    }

    private void Script(string key, StringBuilder source, RenderParams renderParams, IEnumerable<ReplacementDependency> replacements) =>
        RegexReplace(source: source, matchExpression: "\\[script\\[[A-Za-z\\d_/. \\-]*\\]\\]", action: match =>
                                                                                                                               {
                                                                                                                                   string name = match.Value.Replace(oldValue: "[script[", newValue: "")
                                                                                                                                       .Replace(oldValue: "]]", newValue: "")
                                                                                                                                       .ToLower();

                                                                                                                                   Script script = objectCache.Get<Script>(key: "script|" + name);

                                                                                                                                   if (script != null)
                                                                                                                                   {
                                                                                                                                       Script obj = renderParams.App?.Scripts?.FirstOrDefault(predicate: (Script s) => s.Name.Equals(value: name, comparisonType: StringComparison.CurrentCultureIgnoreCase));
                                                                                                                                       return ProcessContentString(key: key, renderParams: renderParams, content: obj?.Content ?? script.Content, replacements: replacements);
                                                                                                                                   }

                                                                                                                                   return string.Empty;
                                                                                                                               });

    private void Component(string key, RenderParams renderParams, IEnumerable<ReplacementDependency> replacements, StringBuilder result) =>
        RegexReplace(source: result, matchExpression: "\\[TYPE\\[[A-Za-z\\d_/-]*\\][A-Za-z\\d_/-]*\\=*\\\"*-*[A-Za-z\\d_/-]*\\\"*\\]".Replace(oldValue: "TYPE", newValue: "component"), action: match =>
                                                                                                                                  {
                                                                                                                                      (string _, string name, string[] options) tag = SplitMatch(match: match);
                                                                                                                                      Component component = renderParams.App?.Components?.FirstOrDefault(predicate: (Component c) => c.Name.Equals(value: tag.name, comparisonType: StringComparison.CurrentCultureIgnoreCase)) ?? objectCache.Get<Component>(key: "component|" + tag.name);
                                                                                                                                      return (component == null) ? ("[[Missing Component:" + tag.name + "]]") : ProcessContentString(key: key, renderParams: renderParams, content: BuildComponentMarkup(component: component, tag: tag, replacements: replacements, renderParams: renderParams), replacements: replacements);
                                                                                                                                  });

    private string BuildComponentMarkup(Component component, (string type, string name, string[] options) tag, IEnumerable<ReplacementDependency> replacements, RenderParams renderParams)
    {
        string value = string.Join(separator: " ", values: tag.options
            .Where(predicate: option => option.StartsWith(value: "class="))
            .Select(selector: option => option.Replace(oldValue: "class=", newValue: "")));

        string content = $"<section name='{component.Name}' class='component {value}' data-id='{component.Id}' data-resource-key='{component.ResourceKey}' {string.Join(separator: " ", values: tag.options.Where(predicate: (string option) => !option.StartsWith(value: "class=")))}>\r\n                        {ProcessContentString(key: component.ResourceKey, renderParams: renderParams, content: component.Content, replacements: replacements)}\r\n                        <script type='text/javascript'>{ProcessContentString(key: component.ResourceKey, renderParams: renderParams, content: component.Script, replacements: replacements)}</script>\r\n                    </section>";
        return ProcessContentString(key: component.ResourceKey, renderParams: renderParams, content: content, replacements: replacements);
    }

    private void ExecuteAsync(string key, StringBuilder source, RenderParams renderParams, IEnumerable<ReplacementDependency> replacements) =>
        RegexReplace(source: source, matchExpression: "\\[execute\\](.*?)\\[/execute\\]", action: match =>
                                                                                                                                     {
                                                                                                                                         string value = match.Groups[1].Value;

                                                                                                                                         string content = SerializeForOData(model: new
                                                                                                                                         {
                                                                                                                                             Script = value,
                                                                                                                                             Model = jsonBroker.ParseJson(json: replacements.First(predicate: (ReplacementDependency r) => r.Old == "[model]")
                                                                                                                                             .New)
                                                                                                                                         });

                                                                                                                                         string result = workflowExecutionBroker.Execute(
                                                                                                                                             baseAddress: replacements.First(predicate: replacement => replacement.Old == "[api[workflow]]").New,
                                                                                                                                             content: content);

                                                                                                                                         return ProcessContentString(key: key, renderParams: renderParams, content: result, replacements: replacements);
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

    private void Resource(string key, StringBuilder source, RenderParams renderParams, IEnumerable<ReplacementDependency> replacements)
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
        Resource resource = objectCache.Get<Resource>(key: $"resource|{key}-{name}-{culture}");

        if (resource != null)
        {
            return resource;
        }

        if (culture.Contains(value: '-'))
        {
            string value = culture.Split(separator: "-")[0];
            Resource resource2 = objectCache.Get<Resource>(key: $"resource|{key}-{name}-{value}");

            if (resource2 != null)
            {
                return resource2;
            }
        }

        return objectCache.Get<Resource>(key: $"resource|{key}-{name}-{string.Empty}");
    }

    private static IEnumerable<Resource> SelectResourcesForCulture(IEnumerable<Resource> potentials, string key, string culture)
    {
        List<Resource> list = new List<Resource>();

        foreach (IGrouping<string, Resource> item in potentials
            .Where(predicate: resource => string.Equals(a: resource.Key, b: key, comparisonType: StringComparison.OrdinalIgnoreCase))
            .GroupBy(keySelector: resource => resource.Name.ToLowerInvariant()))
        {
            Resource closestCulturalMatch = GetClosestCulturalMatch(potentials: item, culture: culture);

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

    private void Meta(StringBuilder source, string culture) =>
        RegexReplace(source: source, matchExpression: "\\[TYPE\\[[A-Za-z\\d_/-]*\\][A-Za-z\\d_/-]*\\=*\\\"*-*[A-Za-z\\d_/-]*\\\"*\\]".Replace(oldValue: "TYPE", newValue: "meta"), action: match =>
                                                               {
                                                                   string value = match.Value;
                                                                   string text = value.Substring(startIndex: 6, length: value.Length - 6);
                                                                   string key = text[..text.IndexOf(value: ']')].ToLowerInvariant();
                                                                   return metadataCache.Get(key: key, culture: culture);
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

    private IEnumerable<ReplacementDependency> BuildThemeReplacements<T>(T model, string prefix = "")
    {
        if ((object)model.GetType()
            .GetInterface(name: "IDynamicMetaObjectProvider") != null && !(model is JObject))
        {
            return BuildDynamicThemeReplacements(model: model, prefix: prefix);
        }

        if (model is JObject)
        {
            return BuildJObjectThemeReplacements(model: model, prefix: prefix);
        }

        if (model is string)
        {
            return new[] { new ReplacementDependency(old: "[theme[" + prefix + "]]", @new: model.ToString()) };
        }

        if (!(model is IEnumerable))
        {
            return BuildIEnumerableThemeReplacements(model: model, prefix: prefix);
        }

        return BuildObjectThemeReplacements(model: model, prefix: prefix);
    }

    private List<ReplacementDependency> BuildObjectThemeReplacements<T>(T model, string prefix)
    {
        string text = prefix ?? string.Empty;
        List<ReplacementDependency> list = new List<ReplacementDependency>();
        int num = 0;

        foreach (object item in (IEnumerable)(object)model)
        {
            string prefix2 = text + $"[{num}]";
            list.AddRange(collection: BuildThemeReplacements(model: item, prefix: prefix2));
            num++;
        }

        string text2 = ((text.Length == 0) ? "Length" : (text + ".Length"));
        list.Add(item: new ReplacementDependency(old: "[theme[" + text2 + "]]", @new: num.ToString()));
        return list;
    }

    private IEnumerable<ReplacementDependency> BuildIEnumerableThemeReplacements<T>(T model, string prefix) =>
        model.GetType()
        .GetProperties()
        .SelectMany(selector: property =>
            {
                object value = property.GetValue(obj: model);
                string text = ((prefix.Length > 0) ? (prefix + "." + property.Name) : property.Name);

                if (property.PropertyType.IsValueType || property.PropertyType == typeof(string))
                {
                    ReplacementDependency[] array = new ReplacementDependency[2];
                    string old = "[theme[" + prefix + "]]";
                    object obj = model?.ToString();

                    if (obj == null)
                    {
                        obj = string.Empty;
                    }

                    array[0] = new ReplacementDependency(old: old, @new: (string)obj);
                    array[1] = new ReplacementDependency(old: "[theme[" + text + "]]", @new: value?.ToString() ?? string.Empty);
                    return array;
                }

                IEnumerable<ReplacementDependency> result;

                if (value == null)
                {
                    IEnumerable<ReplacementDependency> enumerable = Array.Empty<ReplacementDependency>();
                    result = enumerable;
                }
                else
                {
                    result = BuildThemeReplacements(model: value, prefix: text);
                }

                return result;
            })
        .Where(predicate: replacement => replacement.Old != null && replacement.New != null);

    private IEnumerable<ReplacementDependency> BuildJObjectThemeReplacements<T>(T model, string prefix)
    {
        IEnumerable<KeyValuePair<string, JToken>> source = (IEnumerable<KeyValuePair<string, JToken>>)(object)model;

        return source.SelectMany(selector: token =>
        {
            string text = ((prefix.Length > 0) ? (prefix + "." + token.Key) : token.Key);

            if (token.Value.GetType() == typeof(JValue))
            {
                return new[] { new ReplacementDependency(old: "[theme[" + text + "]]", @new: token.Value.ToString() ?? string.Empty) };
            }

            IEnumerable<ReplacementDependency> result;

            if (token.Value == null)
            {
                IEnumerable<ReplacementDependency> enumerable = Array.Empty<ReplacementDependency>();
                result = enumerable;
            }
            else
            {
                result = BuildThemeReplacements(model: token.Value, prefix: text);
            }

            return result;
        });
    }

    private IEnumerable<ReplacementDependency> BuildDynamicThemeReplacements<T>(T model, string prefix)
    {
        IDictionary<string, object> dynamicModel = (IDictionary<string, object>)(object)model;

        return dynamicModel.Keys.SelectMany(selector: key =>
        {
            string text = ((prefix.Length > 0) ? (prefix + "." + key) : key);
            int num = 1;
            List<ReplacementDependency> list = new List<ReplacementDependency>(capacity: num);
            CollectionsMarshal.SetCount(list: list, count: num);
            CollectionsMarshal.AsSpan(list: list)[0] = new ReplacementDependency(old: "[theme[" + text + "]]", @new: dynamicModel[key]?.ToString() ?? string.Empty);
            List<ReplacementDependency> list2 = list;

            if (dynamicModel[key] != null && !dynamicModel[key].GetType()
                .IsValueType)
            {
                list2.AddRange(collection: BuildThemeReplacements(model: dynamicModel[key], prefix: text));
            }

            return list2;
        });
    }

    private IEnumerable<ReplacementDependency> BuildModelReplacements(object model, string prefix = "")
    {
        if (model is string)
        {
            return new[] { new ReplacementDependency(old: "[theme[" + prefix + "]]", @new: model.ToString()) };
        }

        if (model is JObject)
        {
            return BuildModelReplacementsForJObject(model: model, prefix: prefix);
        }

        if (model is JArray)
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

    private IEnumerable<ReplacementDependency> BuildModelReplacementsForCollection(object model, string prefix)
    {
        string text = prefix ?? string.Empty;
        List<ReplacementDependency> list = new List<ReplacementDependency>();
        int num = 0;

        foreach (object item in (IEnumerable)model)
        {
            string prefix2 = text + $"[{num}]";
            list.AddRange(collection: BuildModelReplacements(model: item, prefix: prefix2));
            num++;
        }

        string text2 = ((text.Length == 0) ? "Length" : (text + ".Length"));
        list.Add(item: new ReplacementDependency(old: "[model[" + text2 + "]]", @new: num.ToString()));
        return list;
    }

    private IEnumerable<ReplacementDependency> BuildModelReplacementsForObject(object model, string prefix) =>
        model.GetType()
        .GetProperties()
        .SelectMany(selector: property =>
            {
                object value = property.GetValue(obj: model);
                string text = ((prefix.Length > 0) ? (prefix + "." + property.Name) : property.Name);

                if (property.PropertyType.IsValueType || property.PropertyType == typeof(string))
                {
                    return new ReplacementDependency[2]
                    {
                        new ReplacementDependency(old: "[model[" + prefix + "]]", @new: model?.ToString() ?? string.Empty),
                        new ReplacementDependency(old: "[model[" + text + "]]", @new: value?.ToString() ?? string.Empty)
                    };
                }

                IEnumerable<ReplacementDependency> result;

                if (value == null)
                {
                    IEnumerable<ReplacementDependency> enumerable = Array.Empty<ReplacementDependency>();
                    result = enumerable;
                }
                else
                {
                    result = BuildModelReplacements(model: value, prefix: text);
                }

                return result;
            })
        .Where(predicate: replacement => replacement.Old != null && replacement.New != null)
        .ToList();

    private IEnumerable<ReplacementDependency> BuildModelReplacementsForJObject(object model, string prefix)
    {
        IEnumerable<KeyValuePair<string, JToken>> source = (IEnumerable<KeyValuePair<string, JToken>>)model;

        return source.SelectMany(selector: token =>
        {
            string text = ((prefix.Length > 0) ? (prefix + "." + token.Key) : token.Key);

            if (token.Value.GetType() == typeof(JValue))
            {
                return new[] { new ReplacementDependency(old: "[model[" + text + "]]", @new: token.Value.ToString() ?? string.Empty) };
            }

            IEnumerable<ReplacementDependency> result;

            if (token.Value == null)
            {
                IEnumerable<ReplacementDependency> enumerable = Array.Empty<ReplacementDependency>();
                result = enumerable;
            }
            else
            {
                result = BuildModelReplacements(model: token.Value, prefix: text);
            }

            return result;
        })
            .ToList();
    }

    private IEnumerable<ReplacementDependency> BuildModelReplacementsForDynamicObject(object model, string prefix)
    {
        IDictionary<string, object> dynamicModel = (IDictionary<string, object>)model;

        return dynamicModel.Keys.SelectMany(selector: key =>
        {
            string text = ((prefix.Length > 0) ? (prefix + "." + key) : key);
            int num = 1;
            List<ReplacementDependency> list = new List<ReplacementDependency>(capacity: num);
            CollectionsMarshal.SetCount(list: list, count: num);
            CollectionsMarshal.AsSpan(list: list)[0] = new ReplacementDependency(old: "[model[" + text + "]]", @new: dynamicModel[key]?.ToString() ?? string.Empty);
            List<ReplacementDependency> list2 = list;

            if (dynamicModel[key] != null && !dynamicModel[key].GetType()
                .IsValueType)
            {
                list2.AddRange(collection: BuildModelReplacements(model: dynamicModel[key], prefix: text));
            }

            return list2;
        })
            .ToList();
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

    private static void RegexReplace(StringBuilder source, string matchExpression, Func<Match, string> action)
    {
        Regex regex = new(pattern: matchExpression, options: RegexOptions.Singleline | RegexOptions.IgnoreCase);
        string result = regex.Replace(input: source.ToString(), evaluator: match => action(arg: match));
        source.Clear();
        source.Append(value: result);
    }

    private static void RegexMatch(StringBuilder source, string matchExpression, Action<Match> action)
    {
        MatchCollection matches = Regex.Matches(input: source.ToString(), pattern: matchExpression, options: RegexOptions.Singleline | RegexOptions.IgnoreCase);

        foreach (Match match in matches)
        {
            action(obj: match);
        }
    }

    private static string GetTagName(Match source) =>
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
        ContentManagementConfiguration config,
        ILogger log = null)
    {
        ValidateTemplate(template: template, parameterName: "template");
        ValidateModel(model: model, parameterName: "model");
        ValidateRenderParamsArgument(renderParams: renderParams, parameterName: "renderParams");

        List<ReplacementDependency> list = DefaultReplacements(renderParams: renderParams, config: config)
            .ToList();

        list.Add(item: new ReplacementDependency(old: "[model]", @new: jsonBroker.Serialize(value: model)));
        list.AddRange(collection: BuildModelReplacements(model: model));

        if (log != null && log.IsEnabled(logLevel: LogLevel.Debug))
        {
            log.LogDebug(
                message: "Rendering template {Template} with {ReplacementCount} replacements.",
                args: [template.Name, list.Count]);
        }

        return ProcessContentString(key: template.ResourceKey, renderParams: renderParams, content: template.RawString, replacements: list);
    }
}