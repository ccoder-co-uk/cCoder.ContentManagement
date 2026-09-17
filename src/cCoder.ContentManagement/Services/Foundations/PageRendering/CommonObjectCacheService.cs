// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Caching;
using cCoder.ContentManagement.Models.Caching;

namespace cCoder.ContentManagement.Rendering.Services.Foundations;

internal sealed partial class CommonObjectCacheService(
    ICacheBroker cacheBroker) : ICommonObjectCacheService
{
    private const string CacheKey = "ContentManagement.CommonObjects";

    public CommonObjectCacheSnapshot GetCommonObjectCacheSnapshot() =>
        TryCatch(operation: () => cacheBroker.Get<CommonObjectCacheSnapshot>(
            key: CacheKey));

    public void SetCommonObjectCacheSnapshot(
        CommonObjectCacheSnapshot commonObjectCacheSnapshot,
        TimeSpan expiry) =>
        TryCatch(operation: () =>
        {
            ValidateCommonObjectCacheSnapshotOnSet(
                inputs: [commonObjectCacheSnapshot, expiry]);

            cacheBroker.Set(
                key: CacheKey,
                value: commonObjectCacheSnapshot,
                expiry: expiry);
        });
}