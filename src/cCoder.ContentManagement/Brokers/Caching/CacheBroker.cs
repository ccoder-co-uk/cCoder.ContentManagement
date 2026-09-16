// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;
using cCoder.CodeAnalysis.Exposures;
using Microsoft.Extensions.Caching.Memory;

namespace cCoder.ContentManagement.Brokers.Caching;

internal sealed class CacheBroker(
    MemoryCacheDependency memoryCacheDependency) : ICacheBroker, IUtilityBroker
{
    public T Get<T>(string key) =>
        memoryCacheDependency.Get<T>(key: key);

    public void Set<T>(string key, T value, TimeSpan expiry) =>
        memoryCacheDependency.Set(
            key: key,
            value: value,
            absoluteExpirationRelativeToNow: expiry);
}