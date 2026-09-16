// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Caching;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Models.Caching;
using cCoder.Data;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Rendering.Services.Foundations;

internal sealed partial class MetadataCacheSourceService(
    IMetadataTypeCacheBroker metadataTypeCacheBroker,
    IJsonBroker jsonBroker,
    ICacheBroker cacheBroker) : IMetadataCacheSourceService
{
    private const string CommonObjectCacheKey = "ContentManagement.CommonObjects";

    public MetadataCacheSnapshot BuildMetadataCacheSnapshot() =>
        TryCatch(operation: () =>
        {
            Dictionary<string, IDictionary<string, string>> serialized = new(
                comparer: StringComparer.OrdinalIgnoreCase);

            Resource[] resources = (cacheBroker
                .Get<CommonObjectCacheSnapshot>(key: CommonObjectCacheKey)?
                .Items?
                .Values ?? [])
                .OfType<Resource>()
                .ToArray();

            MetadataContainerSet[] typeSets = GetTypeSets();

            Dictionary<string, string> allJson = new(
                comparer: StringComparer.OrdinalIgnoreCase);

            Dictionary<string, string> dictionaryJson = new(
                comparer: StringComparer.OrdinalIgnoreCase);

            foreach (Culture culture in Cultures.Known)
            {
                Dictionary<string, string> cultureValues = new(
                    comparer: StringComparer.OrdinalIgnoreCase);

                serialized.Add(key: culture.Id, value: cultureValues);

                foreach (MetadataContainerSet metadataContainerSet in typeSets)
                {
                    MetadataContainerSet localized = LocalizeMetadataContainerSet(
                        metadataContainerSet: metadataContainerSet,
                        culture: culture.Id,
                        resources: resources);

                    foreach (ExtendedMetadataContainer type in localized.Types)
                    {
                        cultureValues[
                            $"{localized.Name}/{type.Name}".ToLowerInvariant()] =
                            jsonBroker.Serialize(value: type);
                    }

                    cultureValues[localized.Name.ToLowerInvariant()] =
                        jsonBroker.Serialize(value: localized);
                }

                allJson[culture.Id] = "[" + string.Join(
                    separator: ',',
                    values: typeSets.Select(
                        selector: typeSet => cultureValues[
                            typeSet.Name.ToLowerInvariant()])) + "]";

                dictionaryJson[culture.Id] = jsonBroker.Serialize(
                    value: cultureValues);
            }

            return new MetadataCacheSnapshot
            {
                Serialized = serialized,
                Signature = ComputeMetadataSignature(),
                AllJson = allJson,
                DictionaryJson = dictionaryJson
            };
        });

    public string GetMetadataSignature() =>
        TryCatch(operation: () => ComputeMetadataSignature());

    private MetadataContainerSet[] GetTypeSets() =>
        metadataTypeCacheBroker.GetAll()
            .Select(selector: payload =>
                jsonBroker.ParseJson<MetadataContainerSet>(json: payload))
            .GroupBy(
                keySelector: typeSet => typeSet.Name,
                comparer: StringComparer.OrdinalIgnoreCase)
            .Select(selector: MergeTypeSetGroup)
            .OrderBy(keySelector: typeSet => typeSet.Name)
            .ToArray();

    private string ComputeMetadataSignature() =>
        string.Join(
            separator: "\u001f",
            values: metadataTypeCacheBroker
                .GetAll()
                .OrderBy(
                    keySelector: payload => payload,
                    comparer: StringComparer.Ordinal));

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
                .LastOrDefault(predicate: uriBase =>
                    !string.IsNullOrWhiteSpace(value: uriBase)),
            Types = typeSets
                .SelectMany(selector: typeSet => typeSet.Types ?? [])
                .GroupBy(
                    keySelector: type => type.ServerTypeName,
                    comparer: StringComparer.OrdinalIgnoreCase)
                .Select(selector: types => types.Last())
                .OrderBy(keySelector: type => type.Name)
                .ToArray()
        };
    }

    private static MetadataContainerSet LocalizeMetadataContainerSet(
        MetadataContainerSet metadataContainerSet,
        string culture,
        IEnumerable<Resource> resources) =>
        new()
        {
            Name = metadataContainerSet.Name,
            UriBase = metadataContainerSet.UriBase,
            Types = metadataContainerSet.Types
                .Select(selector: type => LocalizeMetadataContainer(
                    metadataContainer: type,
                    setName: metadataContainerSet.Name,
                    culture: culture,
                    resources: resources))
                .ToArray()
        };

    private static ExtendedMetadataContainer LocalizeMetadataContainer(
        ExtendedMetadataContainer metadataContainer,
        string setName,
        string culture,
        IEnumerable<Resource> resources)
    {
        string cacheKey = $"{setName}|{metadataContainer.ServerTypeName
            .Split(separator: '.')
            .Last()}";

        Resource resource = FindResource(
            resources: resources,
            key: cacheKey,
            culture: culture);

        return new ExtendedMetadataContainer
        {
            Type = metadataContainer.Type,
            ServerTypeName = metadataContainer.ServerTypeName,
            ServerType = metadataContainer.ServerType,
            IsValueType = metadataContainer.IsValueType,
            IsEntity = metadataContainer.IsEntity,
            IsJoinEntity = metadataContainer.IsJoinEntity,
            HasEndpoint = metadataContainer.HasEndpoint,
            IsSystemManaged = metadataContainer.IsSystemManaged,
            Category = metadataContainer.Category,
            Name = metadataContainer.Name,
            DisplayName = resource?.DisplayName ?? metadataContainer.DisplayName,
            Description = resource?.Description ?? metadataContainer.Description,
            Properties = metadataContainer.Properties
                .Select(selector: property => LocalizeProperty(
                    propertyContainer: property,
                    keyContext: cacheKey,
                    culture: culture,
                    resources: resources))
                .ToArray(),
            Operations = metadataContainer.Operations
        };
    }

    private static PropertyContainer LocalizeProperty(
        PropertyContainer propertyContainer,
        string keyContext,
        string culture,
        IEnumerable<Resource> resources)
    {
        Resource resource = FindResource(
            resources: resources,
            key: $"{keyContext}.{propertyContainer.Name}",
            culture: culture);

        return new PropertyContainer
        {
            Name = propertyContainer.Name,
            Type = propertyContainer.Type,
            ServerType = propertyContainer.ServerType,
            ServerTypeName = propertyContainer.ServerTypeName,
            Template = propertyContainer.Template,
            DisplayName = resource?.DisplayName ?? propertyContainer.DisplayName,
            ShortDisplayName = resource?.ShortDisplayName
                ?? propertyContainer.ShortDisplayName,
            Description = resource?.Description ?? propertyContainer.Description,
            IsGeneric = propertyContainer.IsGeneric,
            IsValueType = propertyContainer.IsValueType,
            IsReadOnly = propertyContainer.IsReadOnly,
            IsRequired = propertyContainer.IsRequired,
            IsSystemManaged = propertyContainer.IsSystemManaged
        };
    }

    private static Resource FindResource(
        IEnumerable<Resource> resources,
        string key,
        string culture)
    {
        Resource[] candidates = resources?
            .Where(predicate: resource => string.Equals(
                a: resource.Key,
                b: key,
                comparisonType: StringComparison.OrdinalIgnoreCase))
            .ToArray()
            ?? [];

        return candidates
            .Where(predicate: resource => string.Equals(
                a: resource.Culture ?? string.Empty,
                b: culture ?? string.Empty,
                comparisonType: StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(
                keySelector: resource => resource.Culture?.Length ?? 0)
            .FirstOrDefault()
            ?? candidates.FirstOrDefault(
                predicate: resource =>
                    string.IsNullOrEmpty(value: resource.Culture));
    }
}