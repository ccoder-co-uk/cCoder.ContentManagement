// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.ContentManagement.Brokers.Caching;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Models.Caching;
using cCoder.Data.Models;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Rendering.Services.Foundations;

internal sealed partial class CommonObjectLatestCacheService(
    ICommonObjectBroker commonObjectBroker,
    IJsonBroker jsonBroker,
    ICacheBroker cacheBroker,
    ContentManagementConfiguration configuration) : ICommonObjectLatestCacheService
{
    private const int DefaultCacheExpiryInMinutes = 30;
    private const string CacheKey = "ContentManagement.CommonObjects";

    public CommonObjectCacheSnapshot LoadCommonObjectCacheSnapshot() =>
        TryCatch(operation: () =>
        {
            CommonObject[] latestSet = commonObjectBroker
                .GetLatestCommonObjectsPaged(pageSize: 500)
                .OrderByDescending(
                    keySelector: commonObject => commonObject.Version)
                .ThenByDescending(
                    keySelector: commonObject => commonObject.Id)
                .GroupBy(
                    keySelector: GetEffectiveCacheIdentity,
                    comparer: StringComparer.OrdinalIgnoreCase)
                .Select(selector: versions => versions.First())
                .Where(predicate: commonObject =>
                    commonObject.Type is "ContentManagement/Component"
                        or "ContentManagement/Resource"
                        or "ContentManagement/Script"
                        or "ContentManagement/Style")
                .ToArray();

            Dictionary<string, object> items = new(
                comparer: StringComparer.OrdinalIgnoreCase);

            foreach (CommonObject commonObject in latestSet)
            {
                object item = Deserialize(commonObject: commonObject);

                if (item is not null)
                {
                    items[GetCacheKey(item: item)] = item;
                }
            }

            CommonObjectCacheSnapshot commonObjectCacheSnapshot = new()
            {
                Items = items,
                LatestSet = latestSet
            };

            cacheBroker.Set(
                key: CacheKey,
                value: commonObjectCacheSnapshot,
                expiry: TimeSpan.FromMinutes(
                    minutes: configuration.CacheExpiry > 0
                        ? configuration.CacheExpiry
                        : DefaultCacheExpiryInMinutes));

            return commonObjectCacheSnapshot;
        });

    public CommonObjectCacheSnapshot GetCommonObjectCacheSnapshot() =>
        TryCatch(operation: () => cacheBroker.Get<CommonObjectCacheSnapshot>(
            key: CacheKey));

    private object Deserialize(CommonObject commonObject) =>
        commonObject.Type switch
        {
            "ContentManagement/Component" =>
                jsonBroker.ParseJson<Component>(json: commonObject.Json),
            "ContentManagement/Resource" =>
                jsonBroker.ParseJson<Resource>(json: commonObject.Json),
            "ContentManagement/Script" =>
                jsonBroker.ParseJson<Script>(json: commonObject.Json),
            "ContentManagement/Style" =>
                jsonBroker.ParseJson<Style>(json: commonObject.Json),
            _ => null
        };

    private static string GetCacheKey(object item) =>
        item switch
        {
            Resource resource =>
                $"resource|{resource.Key ?? string.Empty}-{resource.Name ?? string.Empty}-{resource.Culture ?? string.Empty}".ToLowerInvariant(),
            Component component =>
                $"component|{component.Name}".ToLowerInvariant(),
            Script script =>
                $"script|{script.Name}".ToLowerInvariant(),
            Style style =>
                $"style|{style.Name}".ToLowerInvariant(),
            _ => throw new InvalidOperationException(
                message: $"Unsupported common object cache type '{item.GetType().Name}'.")
        };

    private static string GetEffectiveCacheIdentity(CommonObject commonObject) =>
        commonObject.Type == "ContentManagement/Resource"
            ? $"{commonObject.Type}|{commonObject.Key}|{commonObject.Name}|{commonObject.Culture}"
            : $"{commonObject.Type}|{commonObject.Name}";
}