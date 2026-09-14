// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Extensions.OData;
using cCoder.ContentManagement.Exposures.Caching;
using cCoder.ContentManagement.Rendering.Brokers;
using cCoder.ContentManagement.Models;
using cCoder.Data;
using cCoder.Data.Exposures;
using cCoder.Data.Models.CMS;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace cCoder.ContentManagement.Dependencies.Caching;

internal class MetadataCacheDependency : IMetadataCache
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true,
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };

    private readonly IDictionary<string, IDictionary<string, string>> metaSerialized;
    private readonly IMetadataTypeCache metadataTypeCache;
    private readonly ICommonObjectReaderBroker resourceCache;
    private string metadataSignature;

    public MetadataCacheDependency(
        IMetadataTypeCache metadataTypeCache,
        ICommonObjectReaderBroker resourceCache)
    {
        metaSerialized = new Dictionary<string, IDictionary<string, string>>(
            comparer: StringComparer.OrdinalIgnoreCase);
        this.metadataTypeCache = metadataTypeCache;
        this.resourceCache = resourceCache;
        metadataSignature = string.Empty;
        Rebuild();
    }

    public string GetAll(string culture = "")
    {
        EnsureSynchronized();

        return "[" + string.Join(separator: ',', value: GetTypeSets()
            .Select(selector: typeSet => metaSerialized[culture][typeSet.Name.ToLower()])
            .ToArray()) + "]";
    }

    public void Rebuild()
    {
        metaSerialized.Clear();
        Resource[] resources = resourceCache.GetAll<Resource>();

        foreach (var culture in Cultures.Known)
        {
            metaSerialized.Add(
                key: culture.Id,
                value: new Dictionary<string, string>(
                    comparer: StringComparer.OrdinalIgnoreCase));
            MetadataContainerSet[] typeSets = GetTypeSets();

            foreach (MetadataContainerSet metadataContainerSet in typeSets)
            {
                MetadataContainerSet metadataContainerSet2 = metadataContainerSet.Resource(culture: culture.Id, resources: resources);
                ExtendedMetadataContainer[] types = metadataContainerSet2.Types;

                foreach (ExtendedMetadataContainer extendedMetadataContainer in types)
                {
                    Set(key: metadataContainerSet.Name.ToLower() + "/" + extendedMetadataContainer.Name.ToLower(), value: ToJsonForOData(model: extendedMetadataContainer), culture: culture.Id);
                }

                Set(key: metadataContainerSet2.Name.ToLower(), value: ToJsonForOData(model: metadataContainerSet2), culture: culture.Id);
            }
        }

        metadataSignature = ComputeMetadataSignature();
    }

    public void Set(string key, string value, string culture)
    {
        if (metaSerialized[culture].ContainsKey(key: key))
        {
            metaSerialized[culture][key] = value;
        }
        else
        {
            metaSerialized[culture].Add(item: new KeyValuePair<string, string>(key: key, value: value));
        }
    }

    public string ToJson(string culture)
    {
        EnsureSynchronized();
        return ToJsonForOData(model: metaSerialized[culture]);
    }

    public string Get(string key, string culture)
    {
        EnsureSynchronized();
        return metaSerialized[culture].ContainsKey(key: key) ? metaSerialized[culture][key] : string.Empty;
    }

    private void EnsureSynchronized()
    {
        if (!string.Equals(a: metadataSignature, b: ComputeMetadataSignature(), comparisonType: StringComparison.Ordinal))
        {
            Rebuild();
        }
    }

    private MetadataContainerSet[] GetTypeSets() =>
        metadataTypeCache.GetAll()
        .Select(selector: payload => JsonSerializer.Deserialize<MetadataContainerSet>(
            json: payload,
            options: JsonOptions))
        .GroupBy(keySelector: typeSet => typeSet.Name, comparer: StringComparer.OrdinalIgnoreCase)
        .Select(selector: MergeTypeSetGroup)
        .OrderBy(keySelector: typeSet => typeSet.Name)
        .ToArray();

    private static MetadataContainerSet MergeTypeSetGroup(
        IGrouping<string, MetadataContainerSet> group)
    {
        MetadataContainerSet[] typeSets = group.ToArray();
        MetadataContainerSet lastTypeSet = typeSets.Last();

        return new MetadataContainerSet
        {
            Name = lastTypeSet.Name,
            UriBase = typeSets
                .Select(selector: typeSet => typeSet.UriBase)
            .LastOrDefault(predicate: uriBase => !string.IsNullOrWhiteSpace(value: uriBase)),
            Types = typeSets
                .SelectMany(selector: typeSet => typeSet.Types ?? [])
            .GroupBy(keySelector: type => type.ServerTypeName, comparer: StringComparer.OrdinalIgnoreCase)
            .Select(selector: types => types.Last())
            .OrderBy(keySelector: type => type.Name)
            .ToArray(),
        };
    }

    private string ComputeMetadataSignature() =>
        string.Join(
separator: "\u001f",
values: metadataTypeCache
                .GetAll()
        .OrderBy(keySelector: payload => payload, comparer: StringComparer.Ordinal));

    private static string ToJsonForOData(object model) =>
        JsonSerializer.Serialize(value: model, options: JsonOptions);

    public void Dispose()
    {
    }
}