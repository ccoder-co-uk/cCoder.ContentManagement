// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.OData;
using System;
using System.Collections.Generic;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Caching;
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

            List<Resource> resourceValues = [];

            foreach (object value in cacheBroker
                .Get<CommonObjectCacheSnapshot>(key: CommonObjectCacheKey)?
                .Items?
                .Values ?? [])
            {
                if (value is Resource resource)
                {
                    resourceValues.Add(item: resource);
                }
            }

            Resource[] resources = [.. resourceValues];

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

                List<string> serializedTypeSets = [];

                foreach (MetadataContainerSet typeSet in typeSets)
                {
                    serializedTypeSets.Add(item: cultureValues[
                        typeSet.Name.ToLowerInvariant()]);
                }

                allJson[culture.Id] = "[" + string.Join(
                    separator: ',',
                    values: serializedTypeSets) + "]";

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

    private MetadataContainerSet[] GetTypeSets()
    {
        Dictionary<string, List<MetadataContainerSet>> groups = new(
            comparer: StringComparer.OrdinalIgnoreCase);

        foreach (string payload in metadataTypeCacheBroker.GetAll())
        {
            MetadataContainerSet typeSet =
                jsonBroker.ParseJson<MetadataContainerSet>(json: payload);

            if (!groups.TryGetValue(
                key: typeSet.Name,
                value: out List<MetadataContainerSet> group))
            {
                group = [];
                groups[typeSet.Name] = group;
            }

            group.Add(item: typeSet);
        }

        MetadataContainerSet[] typeSets = new MetadataContainerSet[groups.Count];
        int index = 0;

        foreach (List<MetadataContainerSet> group in groups.Values)
        {
            typeSets[index++] = MergeTypeSetGroup(group: group);
        }

        Array.Sort(
            array: typeSets,
            comparison: (left, right) => string.Compare(
                strA: left.Name,
                strB: right.Name,
                comparisonType: StringComparison.Ordinal));

        return typeSets;
    }

    private string ComputeMetadataSignature()
    {
        List<string> payloads = [];

        foreach (string payload in metadataTypeCacheBroker.GetAll())
        {
            payloads.Add(item: payload);
        }

        payloads.Sort(comparer: StringComparer.Ordinal);

        return string.Join(separator: "\u001f", values: payloads);
    }

    private static MetadataContainerSet MergeTypeSetGroup(
        IReadOnlyList<MetadataContainerSet> group)
    {
        MetadataContainerSet lastTypeSet = group[group.Count - 1];
        string uriBase = null;

        Dictionary<string, ExtendedMetadataContainer> typesByServerName = new(
            comparer: StringComparer.OrdinalIgnoreCase);

        foreach (MetadataContainerSet typeSet in group)
        {
            if (!string.IsNullOrWhiteSpace(value: typeSet.UriBase))
            {
                uriBase = typeSet.UriBase;
            }

            foreach (ExtendedMetadataContainer type in typeSet.Types ?? [])
            {
                typesByServerName[type.ServerTypeName] = type;
            }
        }

        ExtendedMetadataContainer[] types = new ExtendedMetadataContainer[
            typesByServerName.Count];

        int index = 0;

        foreach (ExtendedMetadataContainer type in typesByServerName.Values)
        {
            types[index++] = type;
        }

        Array.Sort(
            array: types,
            comparison: (left, right) => string.Compare(
                strA: left.Name,
                strB: right.Name,
                comparisonType: StringComparison.Ordinal));

        return new MetadataContainerSet
        {
            Name = lastTypeSet.Name,
            UriBase = uriBase,
            Types = types
        };
    }

    private static MetadataContainerSet LocalizeMetadataContainerSet(
        MetadataContainerSet metadataContainerSet,
        string culture,
        IEnumerable<Resource> resources)
    {
        ExtendedMetadataContainer[] types = new ExtendedMetadataContainer[
            metadataContainerSet.Types.Length];

        for (int index = 0; index < metadataContainerSet.Types.Length; index++)
        {
            types[index] = LocalizeMetadataContainer(
                metadataContainer: metadataContainerSet.Types[index],
                setName: metadataContainerSet.Name,
                culture: culture,
                resources: resources);
        }

        return new MetadataContainerSet
        {
            Name = metadataContainerSet.Name,
            UriBase = metadataContainerSet.UriBase,
            Types = types
        };
    }

    private static ExtendedMetadataContainer LocalizeMetadataContainer(
        ExtendedMetadataContainer metadataContainer,
        string setName,
        string culture,
        IEnumerable<Resource> resources)
    {
        int lastSeparator = metadataContainer.ServerTypeName.LastIndexOf(
            value: '.');

        string serverTypeName = lastSeparator < 0
            ? metadataContainer.ServerTypeName
            : metadataContainer.ServerTypeName.Substring(
                startIndex: lastSeparator + 1);

        string cacheKey = $"{setName}|{serverTypeName}";

        Resource resource = FindResource(
            resources: resources,
            key: cacheKey,
            culture: culture);

        List<PropertyContainer> properties = [];

        foreach (PropertyContainer property in metadataContainer.Properties)
        {
            properties.Add(item: LocalizeProperty(
                propertyContainer: property,
                keyContext: cacheKey,
                culture: culture,
                resources: resources));
        }

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
            Properties = properties,
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
        Resource exact = null;
        Resource fallback = null;

        foreach (Resource resource in resources ?? [])
        {
            if (!string.Equals(
                a: resource.Key,
                b: key,
                comparisonType: StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (exact is null && string.Equals(
                a: resource.Culture ?? string.Empty,
                b: culture ?? string.Empty,
                comparisonType: StringComparison.OrdinalIgnoreCase))
            {
                exact = resource;
            }

            if (fallback is null && string.IsNullOrEmpty(value: resource.Culture))
            {
                fallback = resource;
            }
        }

        return exact ?? fallback;
    }
}