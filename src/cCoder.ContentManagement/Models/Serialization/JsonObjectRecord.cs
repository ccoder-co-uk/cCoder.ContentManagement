// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Models.Serialization;

internal sealed class JsonObjectRecord
{
    public string RawText { get; init; }

    public IReadOnlyDictionary<string, string> StringValues { get; init; }

    public IReadOnlyDictionary<string, DateTimeOffset> DateTimeOffsetValues { get; init; }

}