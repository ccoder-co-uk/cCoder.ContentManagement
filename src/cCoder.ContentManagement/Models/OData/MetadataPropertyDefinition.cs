// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Models.OData;

internal sealed class MetadataPropertyDefinition
{
    public string Name { get; init; }
    public string Type { get; init; }
    public string ServerType { get; init; }
    public string ServerTypeName { get; init; }
    public bool IsValueType { get; init; }
    public bool IsReadOnly { get; init; }
    public bool IsKey { get; init; }
    public bool IsRequired { get; init; }
}