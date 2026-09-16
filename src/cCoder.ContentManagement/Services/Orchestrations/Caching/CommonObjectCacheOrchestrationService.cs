// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Rendering.Services.Processings;
using cCoder.ContentManagement.Models.Caching;
using cCoder.Data.Models;

namespace cCoder.ContentManagement.Services.Orchestrations.Caching;

internal sealed partial class CommonObjectCacheOrchestrationService(
    ICommonObjectCacheProcessingService cacheProcessingService,
    ICommonObjectLatestCacheProcessingService latestCacheProcessingService)
        : ICommonObjectCacheOrchestrationService
{
    public void Refresh() =>
        TryCatch(operation: () => RefreshCore());

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
}