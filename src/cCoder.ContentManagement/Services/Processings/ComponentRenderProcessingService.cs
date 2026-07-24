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
using cCoder.ContentManagement.Exposures.Caching;
using cCoder.ContentManagement.Services.Foundations;
using cCoder.ContentManagement.Services.Foundations.Storages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using cCoder.ContentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class ComponentRenderProcessingService(
    IMetadataCache metadataCache,
    ICommonObjectCache objectCache,
    IJsonBroker jsonBroker,
    Config config,
    IRenderFileContentService renderFileContentService,
    IAppService appService = null,
    IComponentService componentService = null,
    IResourceService resourceService = null,
    IScriptService scriptService = null) : IComponentRenderProcessingService
{
    private const string TagPattern = "\\[TYPE\\[[A-Za-z\\d_/-]*\\][A-Za-z\\d_/-]*\\=*\\\"*-*[A-Za-z\\d_/-]*\\\"*\\]";

    public string RenderUser(int appId, string name, User user, string culture, string theme) =>
        TryCatch<string>(operation: () =>
    {
        ValidateRenderUser(inputs: [appId, name, user, culture, theme]);
        ValidateAppId(appId: appId, parameterName: "appId");
        ValidateName(name: name, parameterName: "name");
        ValidateTheme(theme: theme, parameterName: "theme");
        ValidateUser(user: user, parameterName: "user");
        EnsureRenderDependenciesConfigured();

        culture ??= user.DefaultCultureId;

        App app = appService.GetAllApp(ignoreFilters: true)
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
            app.Components = componentService.GetAllComponent(ignoreFilters: true)
                .Where(predicate: existingComponent => existingComponent.AppId == appId)
                .ToArray();

            app.Resources = resourceService.GetAllResource(ignoreFilters: true)
                .Where(predicate: existingResource => existingResource.AppId == appId)
                .ToArray();

            app.Scripts = scriptService.GetAllScript(ignoreFilters: true)
                .Where(predicate: existingScript => existingScript.AppId == appId)
                .ToArray();
        }

        Component component = app?.Components
            .Where(predicate: existingComponent => existingComponent.AppId == appId)
            .FirstOrDefault(predicate: existingComponent =>
                existingComponent.Name.Equals(value: name, comparisonType: StringComparison.OrdinalIgnoreCase))
            ?? objectCache.Get<Component>(key: "component|" + name.ToLower())
            ?? throw new InvalidOperationException(message: "Component '" + name + "' was not found.");

        ComponentRenderParams renderParams = new(theme: theme, app: app, user: user, culture: culture);
        return ExecuteRenderComponentComponentRenderParams(component: component, renderParams: renderParams);

    });

    public string RenderComponentComponentRenderParams(Component component, ComponentRenderParams renderParams) =>
        TryCatch<string>(operation: () =>
    {
        ValidateRenderComponentComponentRenderParams(inputs: [component, renderParams]);
        ValidateComponent(component: component, parameterName: "component");
        ValidateComponentRenderParams(renderParams: renderParams, parameterName: "renderParams");
        ICollection<Replacement> replacements = DefaultReplacements(renderParams: renderParams);
        return $"<section name='{component.Name}' class='component' data-id='{component.Id}' data-resource-key='{component.ResourceKey}'>{ProcessContentString(key: component.ResourceKey, renderParams: renderParams, content: component.Content, replacements: replacements)}<script type='text/javascript'>{ProcessContentString(key: component.ResourceKey, renderParams: renderParams, content: component.Script, replacements: replacements)}</script></section>";

    });

    private ICollection<Replacement> DefaultReplacements(RenderParams renderParams)
    {
        ValidateRenderParams(renderParams: renderParams, replacements: null);

        if (renderParams.Culture == null)
        {
            string text = (renderParams.Culture = string.Empty);
        }

        string text2 = (string.IsNullOrEmpty(value: renderParams.Culture) ? renderParams.App.DefaultCultureId : renderParams.Culture);
        string value;
        string text3 = ((config != null && config.Settings.TryGetValue(key: "sslPort", value: out value)) ? (":" + value) : string.Empty);
        int num = 10;
        List<Replacement> list = new List<Replacement>(capacity: num);
        CollectionsMarshal.SetCount(list: list, count: num);
        Span<Replacement> span = CollectionsMarshal.AsSpan(list: list);

        span[0] = new Replacement(old: "[[user]]", @new: jsonBroker.Serialize(value: new
        {
            Id = renderParams.User?.Id,
            DefaultCultureId = renderParams.User?.DefaultCultureId,
            DisplayName = renderParams.User?.DisplayName,
            Email = renderParams.User?.Email
        }));

        span[1] = new Replacement(old: "[[displayname]]", @new: renderParams.User?.DisplayName);
        span[2] = new Replacement(old: "[[loginlink]]", @new: (renderParams.User?.Id == "Guest") ? "<a href='/Login'>[resource_displayname[Login]]</a>" : "<a name='logout' href=''>[resource_displayname[Logout]]</a>");
        span[3] = new Replacement(old: "[[date]]", @new: DateTimeOffset.UtcNow.ToString(format: "dd MMM yyyy"));
        span[4] = new Replacement(old: "[[culture]]", @new: text2);

        span[5] = new Replacement(old: "[[lang]]", @new: text2.Split(separator: '-')
            .First());

        span[6] = new Replacement(old: "[app[name]]", @new: renderParams.App?.Name);
        span[7] = new Replacement(old: "[app[domain]]", @new: renderParams.App?.Domain);
        span[8] = new Replacement(old: "[app[root]]", @new: "https://" + renderParams.App?.Domain + text3 + "/");
        span[9] = new Replacement(old: "[app[id]]", @new: renderParams.App?.Id.ToString());
        List<Replacement> list2 = list;

        if (config != null)
        {
            if (config.Services.TryGetValue(key: "Workflow", value: out var value2))
            {
                list2.Add(item: new Replacement(old: "[api[workflow]]", @new: value2));
            }

            list2.Add(item: new Replacement(old: "[api[root]]", @new: "https://" + renderParams.App?.Domain + text3 + "/Api/"));
        }

        if (renderParams is ComponentRenderParams componentRenderParams)
        {
            list2.Add(item: new Replacement(old: "[theme[name]]", @new: componentRenderParams.Theme));
            IDictionary<string, object> dictionary = default(IDictionary<string, object>);
            object value3 = null;

            if ((TryGetThemeDictionary(config: renderParams.App.Config, themeDictionary: out dictionary)) && dictionary.TryGetValue(key: componentRenderParams.Theme, value: out value3))
            {
                list2.AddRange(collection: BuildThemeReplacements(model: value3));
            }
        }

        return list2;
    }

    private string ProcessContentString(string key, RenderParams renderParams, string content, IEnumerable<Replacement> replacements)
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

        foreach (Replacement replacement in replacements)
        {
            result.Replace(oldValue: replacement.Old, newValue: replacement.New);
        }

        return result.ToString();
    }

    private static void ValidateRenderParams(RenderParams renderParams, IEnumerable<Replacement> replacements)
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

    private static (string type, string name, string[] options) SplitMatch(Match match)
    {
        string[] array = match.ToString()
            .Split(separator: "[");

        string[] array2 = array.Last()
            .Split(separator: "]");

        return (type: array[1].ToLower(), name: array2[0].ToLower(), options: array2[1].Split(separator: "|", options: StringSplitOptions.RemoveEmptyEntries));
    }

    private void Component(string key, RenderParams renderParams, IEnumerable<Replacement> replacements, StringBuilder result) =>
        RegexReplace(source: result, matchExpression: "\\[TYPE\\[[A-Za-z\\d_/-]*\\][A-Za-z\\d_/-]*\\=*\\\"*-*[A-Za-z\\d_/-]*\\\"*\\]".Replace(oldValue: "TYPE", newValue: "component"), action: match =>
                                                                                                                                  {
                                                                                                                                      (string _, string name, string[] options) tag = SplitMatch(match: match);
                                                                                                                                      Component component = renderParams.App?.Components?.FirstOrDefault(predicate: (Component c) => c.Name.Equals(value: tag.name, comparisonType: StringComparison.CurrentCultureIgnoreCase)) ?? objectCache.Get<Component>(key: "component|" + tag.name);
                                                                                                                                      return (component == null) ? ("[[Missing Component:" + tag.name + "]]") : ProcessContentString(key: key, renderParams: renderParams, content: BuildComponentMarkup(component: component, tag: tag, replacements: replacements, renderParams: renderParams), replacements: replacements);
                                                                                                                                  });

    private string BuildComponentMarkup(Component component, (string type, string name, string[] options) tag, IEnumerable<Replacement> replacements, RenderParams renderParams)
    {
        string value = string.Join(separator: " ", values: tag.options
            .Where(predicate: option => option.StartsWith(value: "class="))
            .Select(selector: option => option.Replace(oldValue: "class=", newValue: "")));

        string content = $"<section name='{component.Name}' class='component {value}' data-id='{component.Id}' data-resource-key='{component.ResourceKey}' {string.Join(separator: " ", values: tag.options.Where(predicate: (string option) => !option.StartsWith(value: "class=")))}>\r\n                        {ProcessContentString(key: component.ResourceKey, renderParams: renderParams, content: component.Content, replacements: replacements)}\r\n                        <script type='text/javascript'>{ProcessContentString(key: component.ResourceKey, renderParams: renderParams, content: component.Script, replacements: replacements)}</script>\r\n                    </section>";
        return ProcessContentString(key: component.ResourceKey, renderParams: renderParams, content: content, replacements: replacements);
    }

    private void Script(string key, StringBuilder source, RenderParams renderParams, IEnumerable<Replacement> replacements) =>
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

    private void ExecuteAsync(string key, StringBuilder source, RenderParams renderParams, IEnumerable<Replacement> replacements) =>
        RegexReplace(source: source, matchExpression: "\\[execute\\](.*?)\\[/execute\\]", action: match =>
                                                                                                                                     {
                                                                                                                                         string value = match.Groups[1].Value;
                                                                                                                                         string json = replacements.FirstOrDefault(predicate: (Replacement r) => r.Old == "[model]")?.New ?? "{}";

                                                                                                                                         using HttpClient httpClient = new HttpClient(handler: new HttpClientHandler
                                                                                                                                         {
                                                                                                                                             AutomaticDecompression = (DecompressionMethods.GZip | DecompressionMethods.Deflate)
                                                                                                                                         })
                                                                                                                                         {
                                                                                                                                             BaseAddress = new Uri(uriString: replacements.First(predicate: (Replacement r) => r.Old == "[api[workflow]]")
                                                                                                                                             .New),
                                                                                                                                             Timeout = TimeSpan.FromMinutes(minutes: 10L)
                                                                                                                                         };

                                                                                                                                         string content = SerializeForOData(model: new
                                                                                                                                         {
                                                                                                                                             Script = value,
                                                                                                                                             Model = jsonBroker.ParseJson(json: json)
                                                                                                                                         });

                                                                                                                                         Task<string> task = httpClient.PostAsync(requestUri: "ExecuteScript?useDetails=true", content: new StringContent(content: content, encoding: Encoding.UTF8, mediaType: "text/plain"))
                                                                                                                                             .ContinueWith(continuationFunction: (Task<HttpResponseMessage> t) => t.Result.Content.ReadAsStringAsync())
                                                                                                                                             .Unwrap();

                                                                                                                                         task.Wait();
                                                                                                                                         return ProcessContentString(key: key, renderParams: renderParams, content: task.Result, replacements: replacements);
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

    private void Dms(string key, StringBuilder source, ComponentRenderParams renderParams, IEnumerable<Replacement> replacements) =>
        RegexReplace(source: source, matchExpression: "\\[dms\\[[A-Za-z\\d_/. \\-]*\\]\\]", action: match =>
                                                                                                                                     {
                                                                                                                                         string path = match.Value.Replace(oldValue: "[dms[", newValue: "")
                                                                                                                                             .Replace(oldValue: "]]", newValue: "")
                                                                                                                                             .ToLowerInvariant();

                                                                                                                                         string latestTextContent = renderFileContentService.GetLatestTextContent(appId: renderParams.App.Id, path: path);
                                                                                                                                         return string.IsNullOrEmpty(value: latestTextContent) ? string.Empty : ProcessContentString(key: key, renderParams: renderParams, content: latestTextContent, replacements: replacements);
                                                                                                                                     });

    private void Resource(string key, StringBuilder source, RenderParams renderParams, IEnumerable<Replacement> replacements)
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

    private IEnumerable<Replacement> BuildThemeReplacements<T>(T model, string prefix = "")
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
            return new[] { new Replacement(old: "[theme[" + prefix + "]]", @new: model.ToString()) };
        }

        if (!(model is IEnumerable))
        {
            return BuildIEnumerableThemeReplacements(model: model, prefix: prefix);
        }

        return BuildObjectThemeReplacements(model: model, prefix: prefix);
    }

    private List<Replacement> BuildObjectThemeReplacements<T>(T model, string prefix)
    {
        string text = prefix ?? string.Empty;
        List<Replacement> list = new List<Replacement>();
        int num = 0;

        foreach (object item in (IEnumerable)(object)model)
        {
            string prefix2 = text + $"[{num}]";
            list.AddRange(collection: BuildThemeReplacements(model: item, prefix: prefix2));
            num++;
        }

        string text2 = ((text.Length == 0) ? "Length" : (text + ".Length"));
        list.Add(item: new Replacement(old: "[theme[" + text2 + "]]", @new: num.ToString()));
        return list;
    }

    private IEnumerable<Replacement> BuildIEnumerableThemeReplacements<T>(T model, string prefix) =>
        model.GetType()
        .GetProperties()
        .SelectMany(selector: property =>
            {
                object value = property.GetValue(obj: model);
                string text = ((prefix.Length > 0) ? (prefix + "." + property.Name) : property.Name);

                if (property.PropertyType.IsValueType || property.PropertyType == typeof(string))
                {
                    Replacement[] array = new Replacement[2];
                    string old = "[theme[" + prefix + "]]";
                    object obj = model?.ToString();

                    if (obj == null)
                    {
                        obj = string.Empty;
                    }

                    array[0] = new Replacement(old: old, @new: (string)obj);
                    array[1] = new Replacement(old: "[theme[" + text + "]]", @new: value?.ToString() ?? string.Empty);
                    return array;
                }

                IEnumerable<Replacement> result;

                if (value == null)
                {
                    IEnumerable<Replacement> enumerable = Array.Empty<Replacement>();
                    result = enumerable;
                }
                else
                {
                    result = BuildThemeReplacements(model: value, prefix: text);
                }

                return result;
            })
        .Where(predicate: replacement => replacement.Old != null && replacement.New != null);

    private IEnumerable<Replacement> BuildJObjectThemeReplacements<T>(T model, string prefix)
    {
        IEnumerable<KeyValuePair<string, JToken>> source = (IEnumerable<KeyValuePair<string, JToken>>)(object)model;

        return source.SelectMany(selector: token =>
        {
            string text = ((prefix.Length > 0) ? (prefix + "." + token.Key) : token.Key);

            if (token.Value.GetType() == typeof(JValue))
            {
                return new[] { new Replacement(old: "[theme[" + text + "]]", @new: token.Value.ToString() ?? string.Empty) };
            }

            IEnumerable<Replacement> result;

            if (token.Value == null)
            {
                IEnumerable<Replacement> enumerable = Array.Empty<Replacement>();
                result = enumerable;
            }
            else
            {
                result = BuildThemeReplacements(model: token.Value, prefix: text);
            }

            return result;
        });
    }

    private IEnumerable<Replacement> BuildDynamicThemeReplacements<T>(T model, string prefix)
    {
        IDictionary<string, object> dynamicModel = (IDictionary<string, object>)(object)model;

        return dynamicModel.Keys.SelectMany(selector: key =>
        {
            string text = ((prefix.Length > 0) ? (prefix + "." + key) : key);
            int num = 1;
            List<Replacement> list = new List<Replacement>(capacity: num);
            CollectionsMarshal.SetCount(list: list, count: num);
            CollectionsMarshal.AsSpan(list: list)[0] = new Replacement(old: "[theme[" + text + "]]", @new: dynamicModel[key]?.ToString() ?? string.Empty);
            List<Replacement> list2 = list;

            if (dynamicModel[key] != null && !dynamicModel[key].GetType()
                .IsValueType)
            {
                list2.AddRange(collection: BuildThemeReplacements(model: dynamicModel[key], prefix: text));
            }

            return list2;
        });
    }

    public static IEnumerable<Resource> SectionForCultureResource(IEnumerable<Resource> potentials, string key, string culture) =>
        TryCatch<IEnumerable<Resource>>(operation: () =>
    {
        ValidateSectionForCultureResource(inputs: [potentials, key, culture]);
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

    });

    public static Resource GetClosestCulturalMatchResource(IEnumerable<Resource> potentials, string culture) =>
        TryCatch<Resource>(operation: () =>
    {
        ValidateClosestCulturalMatchResourceOnGet(inputs: [potentials, culture]);
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

    });

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

    private void EnsureRenderDependenciesConfigured()
    {
        if (appService == null ||
            componentService == null ||
            resourceService == null ||
            scriptService == null)
        {
            throw new InvalidOperationException(message: "Render storage services are not configured.");
        }
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

    private string ExecuteRenderComponentComponentRenderParams(Component component, ComponentRenderParams renderParams)
    {
        ValidateComponent(component: component, parameterName: "component");
        ValidateComponentRenderParams(renderParams: renderParams, parameterName: "renderParams");
        ICollection<Replacement> replacements = DefaultReplacements(renderParams: renderParams);
        return $"<section name='{component.Name}' class='component' data-id='{component.Id}' data-resource-key='{component.ResourceKey}'>{ProcessContentString(key: component.ResourceKey, renderParams: renderParams, content: component.Content, replacements: replacements)}<script type='text/javascript'>{ProcessContentString(key: component.ResourceKey, renderParams: renderParams, content: component.Script, replacements: replacements)}</script></section>";
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