// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Models.Rendering;

internal sealed class RuntimePropertyValue
{
    public string Name { get; init; }

    public object Value { get; init; }

    public bool IsValueType { get; init; }
}