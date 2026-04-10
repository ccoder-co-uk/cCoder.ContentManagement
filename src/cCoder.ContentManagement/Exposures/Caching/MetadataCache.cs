using cCoder.ContentManagement.Api.OData;
using cCoder.ContentManagement.Models;
using cCoder.Data;
using cCoder.Data.Exposures;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Resource = cCoder.Data.Models.CMS.Resource;

namespace cCoder.ContentManagement.Exposures.Caching;

internal class MetadataCache : IMetadataCache
{
    public int ExpiryTimeInMinutes { get; set; } = 60;

    private readonly IDictionary<string, IDictionary<string, string>> metaSerialized;

    private readonly IMetadataTypeCache metadataTypeCache;

    private readonly ICommonObjectCache resourceCache;

    public MetadataCache(IMetadataTypeCache metadataTypeCache, ICommonObjectCache resourceCache)
    {
        metaSerialized = new Dictionary<string, IDictionary<string, string>>();
        this.metadataTypeCache = metadataTypeCache;
        this.resourceCache = resourceCache;
        Rebuild();
    }

    public string GetAll(string culture = "")
    {
        return "[" + string.Join(',', (from c in GetTypeSets()
                                       select metaSerialized[culture][c.Name.ToLower()]).ToArray()) + "]";
    }

    public void Rebuild()
    {
        metaSerialized.Clear();
        Resource[] resources = resourceCache.GetAll<Resource>();
        foreach (var culture in Cultures.Known)
        {
            metaSerialized.Add(culture.Id, new Dictionary<string, string>());
            MetadataContainerSet[] typeSets = GetTypeSets();
            foreach (MetadataContainerSet metadataContainerSet in typeSets)
            {
                MetadataContainerSet metadataContainerSet2 = metadataContainerSet.Resource(culture.Id, resources);
                ExtendedMetadataContainer[] types = metadataContainerSet2.Types;
                foreach (ExtendedMetadataContainer extendedMetadataContainer in types)
                {
                    Set(metadataContainerSet.Name.ToLower() + "/" + extendedMetadataContainer.Name.ToLower(), ToJsonForOData(extendedMetadataContainer), culture.Id);
                }
                Set(metadataContainerSet2.Name.ToLower(), ToJsonForOData(metadataContainerSet2), culture.Id);
            }
        }
    }

    public void Set(string key, string value, string culture)
    {
        if (metaSerialized[culture].ContainsKey(key))
        {
            metaSerialized[culture][key] = value;
        }
        else
        {
            metaSerialized[culture].Add(new KeyValuePair<string, string>(key, value));
        }
    }

    public string ToJson(string culture)
    {
        return ToJsonForOData(metaSerialized[culture]);
    }

    public string Get(string key, string culture)
    {
        return metaSerialized[culture].ContainsKey(key) ? metaSerialized[culture][key] : string.Empty;
    }

    private MetadataContainerSet[] GetTypeSets()
    {
        return metadataTypeCache.GetAll()
            .Select(payload => JsonConvert.DeserializeObject<MetadataContainerSet>(payload))
            .GroupBy(typeSet => typeSet.Name, StringComparer.OrdinalIgnoreCase)
            .Select(MergeTypeSetGroup)
            .OrderBy(typeSet => typeSet.Name)
            .ToArray();
    }

    private static MetadataContainerSet MergeTypeSetGroup(
        IGrouping<string, MetadataContainerSet> group)
    {
        MetadataContainerSet[] typeSets = group.ToArray();
        MetadataContainerSet lastTypeSet = typeSets.Last();

        return new MetadataContainerSet
        {
            Name = lastTypeSet.Name,
            UriBase = typeSets
                .Select(typeSet => typeSet.UriBase)
                .LastOrDefault(uriBase => !string.IsNullOrWhiteSpace(uriBase)),
            Types = typeSets
                .SelectMany(typeSet => typeSet.Types ?? [])
                .GroupBy(type => type.ServerTypeName, StringComparer.OrdinalIgnoreCase)
                .Select(types => types.Last())
                .OrderBy(type => type.Name)
                .ToArray(),
        };
    }

    private static string ToJsonForOData(object model)
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

    public void Dispose()
    {
    }
}

