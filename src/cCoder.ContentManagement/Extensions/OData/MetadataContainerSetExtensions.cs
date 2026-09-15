// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Extensions.OData;

internal static class MetadataContainerSetExtensions
{
    internal static MetadataContainerSet Resource(
        this MetadataContainerSet metadataContainerSet,
        string culture,
        IEnumerable<Resource> resources) =>
        new()
        {
            Name = metadataContainerSet.Name,
            UriBase = metadataContainerSet.UriBase,
            Types = metadataContainerSet.Types
                .Select(selector: type => ResourceMetadataContainer(
                    metadataContainer: type,
                    setName: metadataContainerSet.Name,
                    culture: culture,
                    resources: resources))
                .ToArray()
        };

    private static ExtendedMetadataContainer ResourceMetadataContainer(
        ExtendedMetadataContainer metadataContainer,
        string setName,
        string culture,
        IEnumerable<Resource> resources)
    {
        string cacheKey =
            $"{setName}|{metadataContainer.ServerTypeName
                .Split(separator: '.')
                .Last()}";

        Resource resource = ForKeyAndCulture(
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
                .Select(selector: property => ResourceProperty(
                    propertyContainer: property,
                    keyContext: cacheKey,
                    culture: culture,
                    resources: resources))
                .ToArray(),
            Operations = metadataContainer.Operations
        };
    }

    private static PropertyContainer ResourceProperty(
        PropertyContainer propertyContainer,
        string keyContext,
        string culture,
        IEnumerable<Resource> resources)
    {
        Resource resource = ForKeyAndCulture(
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
            ShortDisplayName =
                resource?.ShortDisplayName ?? propertyContainer.ShortDisplayName,
            Description = resource?.Description ?? propertyContainer.Description,
            IsGeneric = propertyContainer.IsGeneric,
            IsValueType = propertyContainer.IsValueType,
            IsReadOnly = propertyContainer.IsReadOnly,
            IsRequired = propertyContainer.IsRequired,
            IsSystemManaged = propertyContainer.IsSystemManaged
        };
    }

    private static Resource ForKeyAndCulture(
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