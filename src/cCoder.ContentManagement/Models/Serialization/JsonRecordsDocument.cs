// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Models.Serialization;

internal sealed class JsonRecordsDocument
{
    public string Json { get; init; }

    public IReadOnlyCollection<JsonObjectRecord> Records { get; init; }
}