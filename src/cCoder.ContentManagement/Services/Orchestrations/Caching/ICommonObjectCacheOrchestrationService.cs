// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;

namespace cCoder.ContentManagement.Services.Orchestrations.Caching;

internal interface ICommonObjectCacheOrchestrationService
{
    void Refresh();

    void EnsureAvailable();

    T[] GetAll<T>();

    T Get<T>(string key);

    void Set(string key, object item);

    IEnumerable<CommonObject> GetLatestSet();
}