// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Models.OData;

internal sealed class MetadataTypeDefinition
{
    public bool IsValueType { get; init; }
    public string Type { get; init; }
    public string Name { get; init; }
    public string ServerType { get; init; }
    public string ServerTypeName { get; init; }
    public bool IsJoinEntity { get; init; }
    public MetadataPropertyDefinition[] Properties { get; init; }
}