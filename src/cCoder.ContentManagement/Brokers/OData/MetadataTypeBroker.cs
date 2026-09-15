// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies.OData;
using cCoder.ContentManagement.Models.OData;

namespace cCoder.ContentManagement.Brokers.OData;

internal sealed class MetadataTypeBroker(MetadataTypeDependency metadataTypeDependency = null)
    : IMetadataTypeBroker
{
    private readonly MetadataTypeDependency metadataTypeDependency =
        metadataTypeDependency ?? new MetadataTypeDependency();

    public MetadataTypeDefinition GetDefinition<T>() =>
        metadataTypeDependency.GetDefinition<T>();

    public MetadataTypeDefinition GetDefinition(Type type) =>
        metadataTypeDependency.GetDefinition(type: type);

    public string Serialize(object value) =>
        metadataTypeDependency.Serialize(value: value);
}