// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Caching;
using cCoder.ContentManagement.Models.Caching;

namespace cCoder.ContentManagement.Tests.Brokers.Caching;

#pragma warning disable STXTEST001
internal sealed class TestCacheBroker : ICacheBroker
{
    private readonly Dictionary<string, object> cache =
        new(comparer: StringComparer.OrdinalIgnoreCase);

    private readonly Dictionary<string, object> commonObjects =
        new(comparer: StringComparer.OrdinalIgnoreCase);

    private readonly Dictionary<string, IDictionary<string, string>> metadata =
        new(comparer: StringComparer.OrdinalIgnoreCase);

    public T Get<T>(string key)
    {
        RefreshSnapshots();

        return cache.TryGetValue(key: key, value: out object value)
            && value is T typedValue
                ? typedValue
                : default;
    }

    public void Set<T>(string key, T value, TimeSpan expiry) =>
        cache[key] = value;

    public void SetCommonObject<T>(string key, T value) =>
        commonObjects[key] = value;

    public void SetMetadata(string key, string culture, string value)
    {
        if (!metadata.TryGetValue(
            key: culture,
            value: out IDictionary<string, string> values))
        {
            values = new Dictionary<string, string>(
                comparer: StringComparer.OrdinalIgnoreCase);

            metadata[culture] = values;
        }

        values[key] = value;
    }

    private void RefreshSnapshots()
    {
        cache["ContentManagement.CommonObjects"] =
            new CommonObjectCacheSnapshot
            {
                Items = commonObjects,
                LatestSet = []
            };

        cache["ContentManagement.Metadata"] = new MetadataCacheSnapshot
        {
            Serialized = metadata,
            Signature = string.Empty,
            AllJson = new Dictionary<string, string>(),
            DictionaryJson = new Dictionary<string, string>()
        };
    }
}
#pragma warning restore STXTEST001