// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Extensions.OData;

internal static class ExtendedMetadataContainerExtensions
{
    internal static ExtendedMetadataContainer Resource(
        this ExtendedMetadataContainer metadataContainer,
        string setName,
        string culture,
        IEnumerable<Resource> resources)
    {
        MetadataContainer localizedMetadata =
            ((MetadataContainer)metadataContainer).Resource(
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
}