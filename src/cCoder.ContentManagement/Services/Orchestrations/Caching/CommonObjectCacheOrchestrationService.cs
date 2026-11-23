// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Processings;
using cCoder.ContentManagement.Rendering.Services.Processings;
using cCoder.ContentManagement.Models.Caching;
using cCoder.Data.Models;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Orchestrations.Caching;

internal sealed partial class CommonObjectCacheOrchestrationService(
    ICommonObjectCacheProcessingService cacheProcessingService,
    ICommonObjectLatestCacheProcessingService latestCacheProcessingService,
    IJsonProcessingService jsonProcessingService)
        : ICommonObjectCacheOrchestrationService
{
    public void Refresh() =>
        TryCatch(operation: () => RefreshCore());

    public void ApplyCommonObjects(CommonObject[] changedCommonObjects) =>
        TryCatch(operation: () =>
        {
            Validate(inputs: [changedCommonObjects]);

            CommonObjectCacheSnapshot current =
                cacheProcessingService.GetCommonObjectCacheSnapshot();

            if (current is null)
            {
                RefreshCore();
                return;
            }

            Dictionary<string, object> items = new(
                current.Items,
                comparer: StringComparer.OrdinalIgnoreCase);

            List<CommonObject> latestSet = [.. current.LatestSet];

            foreach (CommonObject changedCommonObject in changedCommonObjects)
            {
                string changedIdentity = GetEffectiveCacheIdentity(
                    commonObject: changedCommonObject);

                latestSet.RemoveAll(match: commonObject => string.Equals(
                    a: GetEffectiveCacheIdentity(commonObject: commonObject),
                    b: changedIdentity,
                    comparisonType: StringComparison.OrdinalIgnoreCase));

                object changedItem = Deserialize(
                    commonObject: changedCommonObject);

                if (changedItem is not null)
                {
                    latestSet.Add(item: changedCommonObject);
                    items[GetCacheKey(item: changedItem)] = changedItem;
                }
            }

            cacheProcessingService.SetCommonObjectCacheSnapshot(
                commonObjectCacheSnapshot: new CommonObjectCacheSnapshot
                {
                    Items = items,
                    LatestSet = [.. latestSet]
                });
        });

    public void EnsureAvailable() =>
        TryCatch(operation: () =>
        {
            if (!cacheProcessingService.IsAvailable())
            {
                RefreshCore();
            }
        });

    public T[] GetAll<T>() =>
        TryCatch(operation: () => cacheProcessingService.GetAll<T>());

    public T Get<T>(string key) =>
        TryCatch(operation: () =>
        {
            Validate(inputs: [key]);
            return cacheProcessingService.Get<T>(key: key);
        });

    public void Set(string key, object item) =>
        TryCatch(operation: () =>
        {
            Validate(inputs: [key, item]);
            cacheProcessingService.Set(key: key, item: item);
        });

    public IEnumerable<CommonObject> GetLatestSet() =>
        TryCatch(operation: () => cacheProcessingService
            .GetCommonObjectCacheSnapshot()?
            .LatestSet
            ?? []);

    private void RefreshCore()
    {
        CommonObjectCacheSnapshot commonObjectCacheSnapshot =
            latestCacheProcessingService.LoadCommonObjectCacheSnapshot();

        cacheProcessingService.SetCommonObjectCacheSnapshot(
            commonObjectCacheSnapshot: commonObjectCacheSnapshot);
    }

    private object Deserialize(CommonObject commonObject) =>
        commonObject.Type switch
        {
            "ContentManagement/Component" => Deserialize<Component>(
                commonObject: commonObject),
            "ContentManagement/Resource" => Deserialize<Resource>(
                commonObject: commonObject),
            "ContentManagement/Script" => Deserialize<Script>(
                commonObject: commonObject),
            "ContentManagement/Style" => Deserialize<Style>(
                commonObject: commonObject),
            _ => null
        };

    private T Deserialize<T>(CommonObject commonObject) =>
        jsonProcessingService.DeserializeItems<T>(
            json: $"[{commonObject.Json}]")
        .FirstOrDefault();

    private static string GetCacheKey(object item) =>
        item switch
        {
            Resource resource =>
                $"resource|{resource.Key ?? string.Empty}-{resource.Name ?? string.Empty}-{resource.Culture ?? string.Empty}".ToLowerInvariant(),
            Component component => $"component|{component.Name}".ToLowerInvariant(),
            Script script => $"script|{script.Name}".ToLowerInvariant(),
            Style style => $"style|{style.Name}".ToLowerInvariant(),
            _ => throw new InvalidOperationException(
                message: "Unsupported common object cache type.")
        };

    private static string GetEffectiveCacheIdentity(CommonObject commonObject) =>
        commonObject.Type == "ContentManagement/Resource"
            ? $"{commonObject.Type}|{commonObject.Key}|{commonObject.Name}|{commonObject.Culture}"
            : $"{commonObject.Type}|{commonObject.Name}";
}