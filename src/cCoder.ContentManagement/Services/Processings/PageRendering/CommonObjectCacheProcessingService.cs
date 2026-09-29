// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Models.Caching;
using cCoder.ContentManagement.Rendering.Services.Foundations;

namespace cCoder.ContentManagement.Rendering.Services.Processings;

internal sealed partial class CommonObjectCacheProcessingService(
    ICommonObjectCacheService commonObjectCacheService,
    ContentManagementConfiguration configuration)
        : ICommonObjectCacheProcessingService
{
    private const int DefaultCacheExpiryInMinutes = 30;

    public bool IsAvailable() =>
        TryCatch(operation: () =>
            commonObjectCacheService
                .GetCommonObjectCacheSnapshot()?
                .LatestSet?
                .Length > 0);

    public T[] GetAll<T>() =>
        TryCatch<T[]>(operation: () =>
        {
            List<T> results = [];

            IEnumerable<object> values = commonObjectCacheService
                .GetCommonObjectCacheSnapshot()?
                .Items?
                .Values ?? [];

            foreach (object value in values)
            {
                if (value is T result)
                {
                    results.Add(item: result);
                }
            }

            return [.. results];
        });

    public T Get<T>(string key) =>
        TryCatch(operation: () =>
        {
            ValidateKeyOnGet(inputs: [key]);

            IReadOnlyDictionary<string, object> items =
                commonObjectCacheService
                    .GetCommonObjectCacheSnapshot()?
                    .Items;

            return items is not null
                && items.TryGetValue(
                    key: key.ToLowerInvariant(),
                    value: out object item)
                ? (T)item
                : default;
        });

    public void Set(string key, object item) =>
        TryCatch(operation: () =>
        {
            ValidateCacheItemOnSet(inputs: [key, item]);

            CommonObjectCacheSnapshot current =
                commonObjectCacheService.GetCommonObjectCacheSnapshot()
                ?? EmptySnapshot();

            Dictionary<string, object> items =
                new(comparer: StringComparer.OrdinalIgnoreCase);

            foreach (KeyValuePair<string, object> pair in current.Items)
            {
                items[pair.Key] = pair.Value;
            }

            items[key.ToLowerInvariant()] = item;

            SetCommonObjectCacheSnapshotCore(
                commonObjectCacheSnapshot: new CommonObjectCacheSnapshot
                {
                    Items = items,
                    LatestSet = current.LatestSet
                });
        });

    public void SetCommonObjectCacheSnapshot(
        CommonObjectCacheSnapshot commonObjectCacheSnapshot) =>
        TryCatch(operation: () =>
        {
            ValidateCommonObjectCacheSnapshotOnSet(
                inputs: [commonObjectCacheSnapshot]);

            SetCommonObjectCacheSnapshotCore(
                commonObjectCacheSnapshot: commonObjectCacheSnapshot);
        });

    public CommonObjectCacheSnapshot GetCommonObjectCacheSnapshot() =>
        TryCatch(operation: () => commonObjectCacheService
            .GetCommonObjectCacheSnapshot());

    private void SetCommonObjectCacheSnapshotCore(
        CommonObjectCacheSnapshot commonObjectCacheSnapshot) =>
        commonObjectCacheService.SetCommonObjectCacheSnapshot(
            commonObjectCacheSnapshot: commonObjectCacheSnapshot,
            expiry: TimeSpan.FromMinutes(
                minutes: configuration.CacheExpiry > 0
                    ? configuration.CacheExpiry
                    : DefaultCacheExpiryInMinutes));

    private static CommonObjectCacheSnapshot EmptySnapshot() =>
        new()
        {
            Items = new Dictionary<string, object>(),
            LatestSet = []
        };
}