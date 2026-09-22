// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Orchestrations.Caching;
using cCoder.Data.Models;

namespace cCoder.ContentManagement.Exposures.Caching;

internal sealed class CommonObjectCacheManager(
    ICommonObjectCacheOrchestrationService orchestrationService)
        : ICommonObjectCache
{
    public void Refresh() =>
        orchestrationService.Refresh();

    public void EnsureAvailable() =>
        orchestrationService.EnsureAvailable();

    public T[] GetAll<T>() =>
        orchestrationService.GetAll<T>();

    public T Get<T>(string key) =>
        orchestrationService.Get<T>(key: key);

    public void Set(string key, object item) =>
        orchestrationService.Set(
            key: key,
            item: item);

    public IEnumerable<CommonObject> GetLatestSet() =>
        orchestrationService.GetLatestSet();
}