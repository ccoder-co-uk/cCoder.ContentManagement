// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Models.OData;

internal static class ODataMetadataDependency
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
                .Select(selector: type => type.Resource(
                    setName: metadataContainerSet.Name,
                    culture: culture,
                    resources: resources))
                .ToArray()
        };

    internal static MetadataContainer Resource(
        this MetadataContainer metadataContainer,
        string setName,
        string culture,
        IEnumerable<Resource> resources)
    {
        string cacheKey =
            $"{setName}|{metadataContainer.ServerTypeName.Split(separator: '.')
                .Last()}";

        Resource resource = MetadataResourceDependency.ForKeyAndCulture(
            resources: resources,
            key: cacheKey,
            culture: culture);

        return new MetadataContainer
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
            DisplayName = resource?.DisplayName
                ?? metadataContainer.DisplayName,
            Description = resource?.Description
                ?? metadataContainer.Description,
            Properties = metadataContainer.Properties
                .Select(selector: property => property.Resource(
                    keyContext: cacheKey,
                    culture: culture,
                    resources: resources))
                .ToArray()
        };
    }

    internal static ExtendedMetadataContainer Resource(
        this ExtendedMetadataContainer metadataContainer,
        string setName,
        string culture,
        IEnumerable<Resource> resources)
    {
        MetadataContainer localizedMetadata = ((MetadataContainer)metadataContainer)
            .Resource(
                setName: setName,
                culture: culture,
                resources: resources);

        return new ExtendedMetadataContainer
        {
            Type = localizedMetadata.Type,
            ServerTypeName = localizedMetadata.ServerTypeName,
            ServerType = localizedMetadata.ServerType,
            IsValueType = localizedMetadata.IsValueType,
            IsEntity = localizedMetadata.IsEntity,
            IsJoinEntity = localizedMetadata.IsJoinEntity,
            HasEndpoint = localizedMetadata.HasEndpoint,
            IsSystemManaged = localizedMetadata.IsSystemManaged,
            Category = localizedMetadata.Category,
            Name = localizedMetadata.Name,
            DisplayName = localizedMetadata.DisplayName,
            Description = localizedMetadata.Description,
            Properties = localizedMetadata.Properties,
            Operations = metadataContainer.Operations
        };
    }

    internal static PropertyContainer Resource(
        this PropertyContainer propertyContainer,
        string keyContext,
        string culture,
        IEnumerable<Resource> resources)
    {
        Resource resource = MetadataResourceDependency.ForKeyAndCulture(
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
            DisplayName = resource?.DisplayName
                ?? propertyContainer.DisplayName,
            ShortDisplayName = resource?.ShortDisplayName
                ?? propertyContainer.ShortDisplayName,
            Description = resource?.Description
                ?? propertyContainer.Description,
            IsGeneric = propertyContainer.IsGeneric,
            IsValueType = propertyContainer.IsValueType,
            IsReadOnly = propertyContainer.IsReadOnly,
            IsRequired = propertyContainer.IsRequired,
            IsSystemManaged = propertyContainer.IsSystemManaged
        };
    }
}
