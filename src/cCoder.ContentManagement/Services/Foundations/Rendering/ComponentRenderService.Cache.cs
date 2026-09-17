// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Caching;

namespace cCoder.ContentManagement.Services.Foundations.Rendering;

internal sealed partial class ComponentRenderService
{
    private T GetCommonObject<T>(string key) =>
        cacheBroker.Get<CommonObjectCacheSnapshot>(
            key: "ContentManagement.CommonObjects")?
            .Items?
            .TryGetValue(key: key, value: out object value) == true
                && value is T typedValue
                    ? typedValue
                    : default;

    private string GetMetadata(string key, string culture) =>
        cacheBroker.Get<MetadataCacheSnapshot>(
            key: "ContentManagement.Metadata")?
            .Serialized?
            .TryGetValue(
                key: culture,
                value: out IDictionary<string, string> values) == true
            && values.TryGetValue(key: key, value: out string value)
                ? value
                : string.Empty;
}