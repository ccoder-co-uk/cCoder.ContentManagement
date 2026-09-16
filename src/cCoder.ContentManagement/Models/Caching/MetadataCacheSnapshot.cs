// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Models.Caching;

internal sealed class MetadataCacheSnapshot
{
    public IDictionary<string, IDictionary<string, string>> Serialized { get; init; }

    public string Signature { get; init; }

    public IReadOnlyDictionary<string, string> AllJson { get; init; }

    public IReadOnlyDictionary<string, string> DictionaryJson { get; init; }
}