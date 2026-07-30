// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Models.OData;

public class ExtendedMetadataContainer : MetadataContainer
{
    public IEnumerable<OperationContainer> Operations { get; set; }

    public ExtendedMetadataContainer() { }

    public ExtendedMetadataContainer(
        Type type,
        bool isEntity = false,
        bool hasEndpoint = false)
        : base(type, isEntity, hasEndpoint) { }
}