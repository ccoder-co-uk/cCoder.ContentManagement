// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Models.Serialization;

internal sealed class JsonValueDocument
{
    public bool IsRawJson { get; init; }

    public string RawJson { get; init; }

    public object Value { get; init; }
}