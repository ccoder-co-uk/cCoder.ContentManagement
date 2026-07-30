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
                .Select(selector: type => type.Resource(
                    setName: metadataContainerSet.Name,
                    culture: culture,
                    resources: resources))
                .ToArray()
        };
}