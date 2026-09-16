// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Caching;
using cCoder.ContentManagement.Models.Caching;

namespace cCoder.ContentManagement.Rendering.Services.Foundations;

internal sealed partial class MetadataRenderCacheService(
    ICacheBroker cacheBroker) : IMetadataRenderCacheService
{
    private const string CacheKey = "ContentManagement.Metadata";

    public Func<string, string> Get(string culture) =>
        TryCatch<Func<string, string>>(operation: () =>
        {
            ValidateCultureOnGet(inputs: [culture]);

            MetadataCacheSnapshot snapshot =
                cacheBroker.Get<MetadataCacheSnapshot>(key: CacheKey);

            return name => snapshot?.Serialized is not null
                && snapshot.Serialized.TryGetValue(
                    key: culture,
                    value: out IDictionary<string, string> values)
                && values.TryGetValue(key: name, value: out string value)
                    ? value
                    : string.Empty;
        });
}