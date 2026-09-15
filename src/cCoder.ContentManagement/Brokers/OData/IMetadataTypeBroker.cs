// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.OData;

namespace cCoder.ContentManagement.Brokers.OData;

internal interface IMetadataTypeBroker
{
    MetadataTypeDefinition GetDefinition<T>();

    MetadataTypeDefinition GetDefinition(Type type);

    string Serialize(object value);
}