using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Exposures.Caching;
using cCoder.ContentManagement.Services.Foundations.Storages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using App = cCoder.Data.Models.CMS.App;
using Component = cCoder.Data.Models.CMS.Component;
using Config = cCoder.ContentManagement.Models.Config;
using RenderParams = cCoder.ContentManagement.Models.RenderParams;
using Replacement = cCoder.ContentManagement.Models.Replacement;
using Resource = cCoder.Data.Models.CMS.Resource;
using Script = cCoder.Data.Models.CMS.Script;
using Template = cCoder.Data.Models.CMS.Template;
using TemplateRenderParams = cCoder.ContentManagement.Models.TemplateRenderParams;
using User = cCoder.Data.Models.Security.User;

namespace cCoder.ContentManagement.Services.Processings;

internal class TemplateRenderProcessingService(
    IMetadataCache metadataCache,
    ICommonObjectCache objectCache,
    IJsonBroker jsonBroker,
    IAppService appService = null,
    IComponentService componentService = null,
    IResourceService resourceService = null,
    IScriptService scriptService = null,
    ITemplateService templateService = null) : ITemplateRenderProcessingService
{
    private const string TagPattern = "\\[TYPE\\[[A-Za-z\\d_/-]*\\][A-Za-z\\d_/-]*\\=*\\\"*-*[A-Za-z\\d_/-]*\\\"*\\]";

    public string Render(int appId, string name, object model, User user, string culture, Config config, ILogger log = null)
    {
        ValidateAppId(appId, "appId");
        ValidateTemplateName(name, "name");
        ValidateModel(model, "model");
        ValidateUser(user, "user");
        EnsureRenderDependenciesConfigured();

        App app = appService.GetAll(ignoreFilters: true)
            .Where(existingApp => existingApp.Id == appId)
            .Select(existingApp => new App
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
            throw new InvalidOperationException($"App '{appId}' was not found.");
        }

        app.Components = componentService.GetAll(ignoreFilters: true)
            .Where(existingComponent => existingComponent.AppId == appId)
            .ToArray();

        app.Resources = resourceService.GetAll(ignoreFilters: true)
            .Where(existingResource => existingResource.AppId == appId)
            .ToArray();

        app.Scripts = scriptService.GetAll(ignoreFilters: true)
            .Where(existingScript => existingScript.AppId == appId)
            .ToArray();

        Template template = templateService.GetAll(ignoreFilters: true)
            .Where(existingTemplate => existingTemplate.AppId == appId)
            .ToArray()
            .FirstOrDefault(existingTemplate =>
                existingTemplate.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException("Template '" + name + "' was not found.");

        TemplateRenderParams templateRenderParams = new(app, user, culture);
        return RenderTemplate(template, model, templateRenderParams, config, log);
    }

    public string RenderTemplate(Template template, object model, RenderParams renderParams, Config config, ILogger log = null)
    {
        ValidateTemplate(template, "template");
        ValidateModel(model, "model");
        ValidateRenderParamsArgument(renderParams, "renderParams");
        List<Replacement> list = DefaultReplacements(renderParams, config).ToList();
        list.Add(new Replacement("[model]", jsonBroker.Serialize(model)));
        list.AddRange(BuildModelReplacements(model));
        if (log != null && log.IsEnabled(LogLevel.Debug))
        {
            log.LogDebug("Rendering template {Template} with {ReplacementCount} replacements.", template.Name, list.Count);
        }
        return ProcessContentString(template.ResourceKey, renderParams, template.RawString, list);
    }

    private ICollection<Replacement> DefaultReplacements(RenderParams renderParams, Config config = null)
    {
        ValidateRenderParams(renderParams, null);
        if (renderParams.Culture == null)
        {
            string text = (renderParams.Culture = string.Empty);
        }
        string text2 = (string.IsNullOrEmpty(renderParams.Culture) ? renderParams.App.DefaultCultureId : renderParams.Culture);
        string value;
        string text3 = ((config != null && config.Settings.TryGetValue("sslPort", out value)) ? (":" + value) : string.Empty);
        int num = 10;
        List<Replacement> list = new List<Replacement>(num);
        CollectionsMarshal.SetCount(list, num);
        Span<Replacement> span = CollectionsMarshal.AsSpan(list);
        span[0] = new Replacement("[[user]]", jsonBroker.Serialize(new
        {
            Id = renderParams.User?.Id,
            DefaultCultureId = renderParams.User?.DefaultCultureId,
            DisplayName = renderParams.User?.DisplayName,
            Email = renderParams.User?.Email
        }));
        span[1] = new Replacement("[[displayname]]", renderParams.User?.DisplayName);
        span[2] = new Replacement("[[loginlink]]", (renderParams.User?.Id == "Guest") ? "<a href='/Login'>[resource_displayname[Login]]</a>" : "<a name='logout' href=''>[resource_displayname[Logout]]</a>");
        span[3] = new Replacement("[[date]]", DateTimeOffset.UtcNow.ToString("dd MMM yyyy"));
        span[4] = new Replacement("[[culture]]", text2);
        span[5] = new Replacement("[[lang]]", text2.Split('-').First());
        span[6] = new Replacement("[app[name]]", renderParams.App?.Name);
        span[7] = new Replacement("[app[domain]]", renderParams.App?.Domain);
        span[8] = new Replacement("[app[root]]", "https://" + renderParams.App?.Domain + text3 + "/");
        span[9] = new Replacement("[app[id]]", renderParams.App?.Id.ToString());
        List<Replacement> list2 = list;
        if (config != null)
        {
            if (config.Services.TryGetValue("Workflow", out var value2))
            {
                list2.Add(new Replacement("[api[workflow]]", value2));
            }
            list2.Add(new Replacement("[api[root]]", "https://" + renderParams.App?.Domain + text3 + "/Api/"));
        }
        if (renderParams is TemplateRenderParams)
        {
            list2.Add(new Replacement("[theme[name]]", "Default"));
            IDictionary<string, object> source = default(IDictionary<string, object>);
            if (TryGetThemeDictionary(renderParams.App.Config, out source) && source.Any())
            {
                list2.AddRange(BuildThemeReplacements(source.First().Value));
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
        ValidateRenderParams(renderParams, replacements);
        StringBuilder result = new StringBuilder(content, content.Length * 4);
        Script(key, result, renderParams, replacements);
        RegexReplace(result, "\\[TYPE\\[[A-Za-z\\d_/-]*\\][A-Za-z\\d_/-]*\\=*\\\"*-*[A-Za-z\\d_/-]*\\\"*\\]".Replace("TYPE", "culturelink"), match => "?culture=" + GetTagName(match));
        Component(key, renderParams, replacements, result);
        Meta(result, renderParams.Culture);
        Resource(key, result, renderParams, replacements);
        ExecuteAsync(key, result, renderParams, replacements);
        replacements.ToList().ForEach(delegate (Replacement replacement)
        {
            result.Replace(replacement.Old, replacement.New);
        });
        return result.ToString();
    }

    private static void ValidateRenderParams(RenderParams renderParams, IEnumerable<Replacement> replacements)
    {
        if (renderParams == null)
        {
            throw new ValidationException("renderParams is required.");
        }
        if (renderParams.App == null)
        {
            throw new ValidationException("renderParams.App is required.");
        }
        if (renderParams.App.Resources == null)
        {
            throw new ValidationException("renderParams.App.Resources is required.");
        }
        if (replacements != null)
        {
        }
    }

    private static (string type, string name, string[] options) SplitMatch(Match match)
    {
        string[] array = match.ToString().Split("[");
        string[] array2 = array.Last().Split("]");
        return (type: array[1].ToLower(), name: array2[0].ToLower(), options: array2[1].Split("|", StringSplitOptions.RemoveEmptyEntries));
    }

    private void Script(string key, StringBuilder source, RenderParams renderParams, IEnumerable<Replacement> replacements)
    {
        RegexReplace(source, "\\[script\\[[A-Za-z\\d_/. \\-]*\\]\\]", delegate (Match match)
        {
            string name = match.Value.Replace("[script[", "").Replace("]]", "").ToLower();
            Script script = objectCache.Get<Script>("script|" + name);
            if (script != null)
            {
            Script obj = renderParams.App?.Scripts?.FirstOrDefault((Script s) => s.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
                return ProcessContentString(key, renderParams, obj?.Content ?? script.Content, replacements);
            }
            return string.Empty;
        });
    }

    private void Component(string key, RenderParams renderParams, IEnumerable<Replacement> replacements, StringBuilder result)
    {
        RegexReplace(result, "\\[TYPE\\[[A-Za-z\\d_/-]*\\][A-Za-z\\d_/-]*\\=*\\\"*-*[A-Za-z\\d_/-]*\\\"*\\]".Replace("TYPE", "component"), delegate (Match match)
        {
            (string _, string name, string[] options) tag = SplitMatch(match);
            Component component = renderParams.App?.Components?.FirstOrDefault((Component c) => c.Name.Equals(tag.name, StringComparison.CurrentCultureIgnoreCase)) ?? objectCache.Get<Component>("component|" + tag.name);
            return (component == null) ? ("[[Missing Component:" + tag.name + "]]") : ProcessContentString(key, renderParams, BuildComponentMarkup(component, tag, replacements, renderParams), replacements);
        });
    }

    private string BuildComponentMarkup(Component component, (string type, string name, string[] options) tag, IEnumerable<Replacement> replacements, RenderParams renderParams)
    {
        string value = string.Join(" ", from c in tag.options
                                        where c.StartsWith("class=")
                                        select c.Replace("class=", ""));
        string content = $"<section name='{component.Name}' class='component {value}' data-id='{component.Id}' data-resource-key='{component.ResourceKey}' {string.Join(" ", tag.options.Where((string option) => !option.StartsWith("class=")))}>\r\n                        {ProcessContentString(component.ResourceKey, renderParams, component.Content, replacements)}\r\n                        <script type='text/javascript'>{ProcessContentString(component.ResourceKey, renderParams, component.Script, replacements)}</script>\r\n                    </section>";
        return ProcessContentString(component.ResourceKey, renderParams, content, replacements);
    }

    private void ExecuteAsync(string key, StringBuilder source, RenderParams renderParams, IEnumerable<Replacement> replacements)
    {
        RegexReplace(source, "\\[execute\\](.*?)\\[/execute\\]", delegate (Match match)
        {
            string value = match.Groups[1].Value;
            using HttpClient httpClient = new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = (DecompressionMethods.GZip | DecompressionMethods.Deflate)
            })
            {
                BaseAddress = new Uri(replacements.First((Replacement r) => r.Old == "[api[workflow]]").New),
                Timeout = TimeSpan.FromMinutes(10L)
            };
            string content = SerializeForOData(new
            {
                Script = value,
                Model = jsonBroker.ParseJson(replacements.First((Replacement r) => r.Old == "[model]").New)
            });
            Task<string> task = httpClient.PostAsync("ExecuteScript?useDetails=true", new StringContent(content, Encoding.UTF8, "text/plain")).ContinueWith((Task<HttpResponseMessage> t) => t.Result.Content.ReadAsStringAsync()).Unwrap();
            task.Wait();
            return ProcessContentString(key, renderParams, task.Result, replacements);
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

    private void Resource(string key, StringBuilder source, RenderParams renderParams, IEnumerable<Replacement> replacements)
    {
        List<Resource> known = new List<Resource>();
        List<string> namesInKey = new List<string>();
        RegexMatch(source, "\\[TYPE\\[[A-Za-z\\d_/-]*\\][A-Za-z\\d_/-]*\\=*\\\"*-*[A-Za-z\\d_/-]*\\\"*\\]".Replace("TYPE", "resource_displayname"), delegate (Match match)
        {
            namesInKey.Add(GetTagName(match));
        });
        RegexMatch(source, "\\[TYPE\\[[A-Za-z\\d_/-]*\\][A-Za-z\\d_/-]*\\=*\\\"*-*[A-Za-z\\d_/-]*\\\"*\\]".Replace("TYPE", "resource_shortdisplayname"), delegate (Match match)
        {
            namesInKey.Add(GetTagName(match));
        });
        RegexMatch(source, "\\[TYPE\\[[A-Za-z\\d_/-]*\\][A-Za-z\\d_/-]*\\=*\\\"*-*[A-Za-z\\d_/-]*\\\"*\\]".Replace("TYPE", "resource_description"), delegate (Match match)
        {
            namesInKey.Add(GetTagName(match));
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
            list2.AddRange(SelectResourcesForCulture(renderParams.App.Resources, key, renderParams.Culture ?? string.Empty));
            IEnumerable<Resource> enumerable = list2;
            collection = enumerable;
        }
        else
        {
            IEnumerable<Resource> enumerable = Array.Empty<Resource>();
            collection = enumerable;
        }
        list.AddRange(collection);
        string key2 = key.ToLowerInvariant();
        string culture = renderParams.Culture.ToLowerInvariant();
        foreach (string item in namesInKey)
        {
            Resource resource = FindResourceInCache(key2, item.ToLowerInvariant(), culture);
            if (resource != null)
            {
                known.Add(resource);
            }
        }
        RegexReplace(source, "\\[TYPE\\[[A-Za-z\\d_/-]*\\][A-Za-z\\d_/-]*\\=*\\\"*-*[A-Za-z\\d_/-]*\\\"*\\]".Replace("TYPE", "resource_displayname"), match => ProcessContentString(key, renderParams, known.FirstOrDefault(resource => resource.Name.Equals(GetTagName(match), StringComparison.CurrentCultureIgnoreCase))?.DisplayName ?? GetTagName(match).ToLower(), replacements));
        RegexReplace(source, "\\[TYPE\\[[A-Za-z\\d_/-]*\\][A-Za-z\\d_/-]*\\=*\\\"*-*[A-Za-z\\d_/-]*\\\"*\\]".Replace("TYPE", "resource_shortdisplayname"), match => ProcessContentString(key, renderParams, known.FirstOrDefault(resource => resource.Name.Equals(GetTagName(match), StringComparison.CurrentCultureIgnoreCase))?.ShortDisplayName ?? GetTagName(match).ToLower(), replacements));
        RegexReplace(source, "\\[TYPE\\[[A-Za-z\\d_/-]*\\][A-Za-z\\d_/-]*\\=*\\\"*-*[A-Za-z\\d_/-]*\\\"*\\]".Replace("TYPE", "resource_description"), match => ProcessContentString(key, renderParams, known.FirstOrDefault(resource => resource.Name.Equals(GetTagName(match), StringComparison.CurrentCultureIgnoreCase))?.Description ?? GetTagName(match).ToLower(), replacements));
    }

    private Resource FindResourceInCache(string key, string name, string culture)
    {
        Resource resource = objectCache.Get<Resource>($"resource|{key}-{name}-{culture}");
        if (resource != null)
        {
            return resource;
        }
        if (culture.Contains('-'))
        {
            string value = culture.Split("-")[0];
            Resource resource2 = objectCache.Get<Resource>($"resource|{key}-{name}-{value}");
            if (resource2 != null)
            {
                return resource2;
            }
        }
        return objectCache.Get<Resource>($"resource|{key}-{name}-{string.Empty}");
    }

    private static IEnumerable<Resource> SelectResourcesForCulture(IEnumerable<Resource> potentials, string key, string culture)
    {
        List<Resource> list = new List<Resource>();
        foreach (IGrouping<string, Resource> item in from resource in potentials
                                                     where string.Equals(resource.Key, key, StringComparison.OrdinalIgnoreCase)
                                                     group resource by resource.Name.ToLowerInvariant())
        {
            Resource closestCulturalMatch = GetClosestCulturalMatch(item, culture);
            if (closestCulturalMatch != null)
            {
                list.Add(closestCulturalMatch);
            }
        }
        return list;
    }

    private static Resource GetClosestCulturalMatch(IEnumerable<Resource> potentials, string culture)
    {
        Resource resource = null;
        List<string> list = (culture ?? string.Empty).ToLowerInvariant().Split('-').ToList();
        int num = list.Count;
        string resultCulture = string.Empty;
        while (resource == null && resultCulture != null)
        {
            resultCulture = string.Join("-", list.Take(num));
            resource = potentials.FirstOrDefault((Resource resource2) => string.Equals(resource2.Culture, resultCulture, StringComparison.OrdinalIgnoreCase));
            num--;
            if (num == 0)
            {
                resultCulture = null;
            }
        }
        return resource ?? potentials.FirstOrDefault((Resource resource2) => string.IsNullOrEmpty(resource2.Culture));
    }

    private void Meta(StringBuilder source, string culture)
    {
        RegexReplace(source, "\\[TYPE\\[[A-Za-z\\d_/-]*\\][A-Za-z\\d_/-]*\\=*\\\"*-*[A-Za-z\\d_/-]*\\\"*\\]".Replace("TYPE", "meta"), delegate (Match match)
        {
            string value = match.Value;
            string text = value.Substring(6, value.Length - 6);
            string key = text[..text.IndexOf(']')].ToLowerInvariant();
            return metadataCache.Get(key, culture);
        });
    }

    private static bool TryGetThemeDictionary(dynamic config, out IDictionary<string, object> themeDictionary)
    {
        themeDictionary = null;
        if (!(config is IDictionary<string, object> dictionary))
        {
            return false;
        }
        if (!dictionary.TryGetValue("Themes", out var value))
        {
            return false;
        }
        themeDictionary = value as IDictionary<string, object>;
        return themeDictionary != null;
    }

    private IEnumerable<Replacement> BuildThemeReplacements<T>(T model, string prefix = "")
    {
        if ((object)model.GetType().GetInterface("IDynamicMetaObjectProvider") != null && !(model is JObject))
        {
            return BuildDynamicThemeReplacements(model, prefix);
        }
        if (model is JObject)
        {
            return BuildJObjectThemeReplacements(model, prefix);
        }
        if (model is string)
        {
            return new[] { new Replacement("[theme[" + prefix + "]]", model.ToString()) };
        }
        if (!(model is IEnumerable))
        {
            return BuildIEnumerableThemeReplacements(model, prefix);
        }
        return BuildObjectThemeReplacements(model, prefix);
    }

    private List<Replacement> BuildObjectThemeReplacements<T>(T model, string prefix)
    {
        string text = prefix ?? string.Empty;
        List<Replacement> list = new List<Replacement>();
        int num = 0;
        foreach (object item in (IEnumerable)(object)model)
        {
            string prefix2 = text + $"[{num}]";
            list.AddRange(BuildThemeReplacements(item, prefix2));
            num++;
        }
        string text2 = ((text.Length == 0) ? "Length" : (text + ".Length"));
        list.Add(new Replacement("[theme[" + text2 + "]]", num.ToString()));
        return list;
    }

    private IEnumerable<Replacement> BuildIEnumerableThemeReplacements<T>(T model, string prefix)
    {
        return from replacement in model.GetType().GetProperties().SelectMany(delegate (PropertyInfo property)
            {
                object value = property.GetValue(model);
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
                    array[0] = new Replacement(old, (string)obj);
                    array[1] = new Replacement("[theme[" + text + "]]", value?.ToString() ?? string.Empty);
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
                    result = BuildThemeReplacements(value, text);
                }
                return result;
            })
               where replacement.Old != null && replacement.New != null
               select replacement;
    }

    private IEnumerable<Replacement> BuildJObjectThemeReplacements<T>(T model, string prefix)
    {
        IEnumerable<KeyValuePair<string, JToken>> source = (IEnumerable<KeyValuePair<string, JToken>>)(object)model;
        return source.SelectMany(delegate (KeyValuePair<string, JToken> token)
        {
            string text = ((prefix.Length > 0) ? (prefix + "." + token.Key) : token.Key);
            if (token.Value.GetType() == typeof(JValue))
            {
                return new[] { new Replacement("[theme[" + text + "]]", token.Value.ToString() ?? string.Empty) };
            }
            IEnumerable<Replacement> result;
            if (token.Value == null)
            {
                IEnumerable<Replacement> enumerable = Array.Empty<Replacement>();
                result = enumerable;
            }
            else
            {
                result = BuildThemeReplacements(token.Value, text);
            }
            return result;
        });
    }

    private IEnumerable<Replacement> BuildDynamicThemeReplacements<T>(T model, string prefix)
    {
        IDictionary<string, object> dynamicModel = (IDictionary<string, object>)(object)model;
        return dynamicModel.Keys.SelectMany(delegate (string key)
        {
            string text = ((prefix.Length > 0) ? (prefix + "." + key) : key);
            int num = 1;
            List<Replacement> list = new List<Replacement>(num);
            CollectionsMarshal.SetCount(list, num);
            CollectionsMarshal.AsSpan(list)[0] = new Replacement("[theme[" + text + "]]", dynamicModel[key]?.ToString() ?? string.Empty);
            List<Replacement> list2 = list;
            if (dynamicModel[key] != null && !dynamicModel[key].GetType().IsValueType)
            {
                list2.AddRange(BuildThemeReplacements(dynamicModel[key], text));
            }
            return list2;
        });
    }

    private IEnumerable<Replacement> BuildModelReplacements(object model, string prefix = "")
    {
        if (model is string)
        {
            return new[] { new Replacement("[theme[" + prefix + "]]", model.ToString()) };
        }
        if (model is JObject)
        {
            return BuildModelReplacementsForJObject(model, prefix);
        }
        if (model is JArray)
        {
            return BuildModelReplacementsForCollection(model, prefix);
        }
        if ((object)model.GetType().GetInterface("IDynamicMetaObjectProvider") != null)
        {
            return BuildModelReplacementsForDynamicObject(model, prefix);
        }
        return (model is IEnumerable) ? BuildModelReplacementsForCollection(model, prefix) : BuildModelReplacementsForObject(model, prefix);
    }

    private IEnumerable<Replacement> BuildModelReplacementsForCollection(object model, string prefix)
    {
        string text = prefix ?? string.Empty;
        List<Replacement> list = new List<Replacement>();
        int num = 0;
        foreach (object item in (IEnumerable)model)
        {
            string prefix2 = text + $"[{num}]";
            list.AddRange(BuildModelReplacements(item, prefix2));
            num++;
        }
        string text2 = ((text.Length == 0) ? "Length" : (text + ".Length"));
        list.Add(new Replacement("[model[" + text2 + "]]", num.ToString()));
        return list;
    }

    private IEnumerable<Replacement> BuildModelReplacementsForObject(object model, string prefix)
    {
        return (from replacement in model.GetType().GetProperties().SelectMany(delegate (PropertyInfo property)
            {
                object value = property.GetValue(model);
                string text = ((prefix.Length > 0) ? (prefix + "." + property.Name) : property.Name);
                if (property.PropertyType.IsValueType || property.PropertyType == typeof(string))
                {
                    return new Replacement[2]
                    {
                        new Replacement("[model[" + prefix + "]]", model?.ToString() ?? string.Empty),
                        new Replacement("[model[" + text + "]]", value?.ToString() ?? string.Empty)
                    };
                }
                IEnumerable<Replacement> result;
                if (value == null)
                {
                    IEnumerable<Replacement> enumerable = Array.Empty<Replacement>();
                    result = enumerable;
                }
                else
                {
                    result = BuildModelReplacements(value, text);
                }
                return result;
            })
                where replacement.Old != null && replacement.New != null
                select replacement).ToList();
    }

    private IEnumerable<Replacement> BuildModelReplacementsForJObject(object model, string prefix)
    {
        IEnumerable<KeyValuePair<string, JToken>> source = (IEnumerable<KeyValuePair<string, JToken>>)model;
        return source.SelectMany(delegate (KeyValuePair<string, JToken> token)
        {
            string text = ((prefix.Length > 0) ? (prefix + "." + token.Key) : token.Key);
            if (token.Value.GetType() == typeof(JValue))
            {
                return new[] { new Replacement("[model[" + text + "]]", token.Value.ToString() ?? string.Empty) };
            }
            IEnumerable<Replacement> result;
            if (token.Value == null)
            {
                IEnumerable<Replacement> enumerable = Array.Empty<Replacement>();
                result = enumerable;
            }
            else
            {
                result = BuildModelReplacements(token.Value, text);
            }
            return result;
        }).ToList();
    }

    private IEnumerable<Replacement> BuildModelReplacementsForDynamicObject(object model, string prefix)
    {
        IDictionary<string, object> dynamicModel = (IDictionary<string, object>)model;
        return dynamicModel.Keys.SelectMany(delegate (string key)
        {
            string text = ((prefix.Length > 0) ? (prefix + "." + key) : key);
            int num = 1;
            List<Replacement> list = new List<Replacement>(num);
            CollectionsMarshal.SetCount(list, num);
            CollectionsMarshal.AsSpan(list)[0] = new Replacement("[model[" + text + "]]", dynamicModel[key]?.ToString() ?? string.Empty);
            List<Replacement> list2 = list;
            if (dynamicModel[key] != null && !dynamicModel[key].GetType().IsValueType)
            {
                list2.AddRange(BuildModelReplacements(dynamicModel[key], text));
            }
            return list2;
        }).ToList();
    }

    private static Template ValidateTemplate(Template template, string parameterName)
    {
        if (template == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
        return template;
    }

    private static object ValidateModel(object model, string parameterName)
    {
        if (model == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
        return model;
    }

    private static RenderParams ValidateRenderParamsArgument(RenderParams renderParams, string parameterName)
    {
        if (renderParams == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
        return renderParams;
    }

    private static void ValidateAppId(int appId, string parameterName)
    {
        if (appId < 1)
        {
            throw new ValidationException(parameterName + " must be greater than 0.");
        }
    }

    private static void ValidateTemplateName(string name, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ValidationException(parameterName + " is required.");
        }
    }

    private static User ValidateUser(User user, string parameterName)
    {
        if (user == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }

        return user;
    }

    private void EnsureRenderDependenciesConfigured()
    {
        if (appService == null ||
            componentService == null ||
            resourceService == null ||
            scriptService == null ||
            templateService == null)
        {
            throw new InvalidOperationException("Render storage services are not configured.");
        }
    }

    private static void RegexReplace(StringBuilder source, string matchExpression, Func<Match, string> action)
    {
        Regex regex = new(matchExpression, RegexOptions.Singleline | RegexOptions.IgnoreCase);
        string result = regex.Replace(source.ToString(), match => action(match));
        source.Clear();
        source.Append(result);
    }

    private static void RegexMatch(StringBuilder source, string matchExpression, Action<Match> action)
    {
        MatchCollection matches = Regex.Matches(source.ToString(), matchExpression, RegexOptions.Singleline | RegexOptions.IgnoreCase);

        foreach (Match match in matches)
        {
            action(match);
        }
    }

    private static string GetTagName(Match source) =>
        source.Value.Split('[')[2].Replace("]", "").ToLowerInvariant();
}
