// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using cCoder.CodeAnalysis.Exposures;
using Microsoft.Extensions.Caching.Memory;

namespace cCoder.ContentManagement.Brokers.Caching;

internal sealed class CacheBroker(
    IMemoryCache memoryCache) : ICacheBroker, IUtilityBroker
{
    public T Get<T>(string key) =>
        memoryCache.Get<T>(key: key);

    public void Set<T>(string key, T value, TimeSpan expiry) =>
        memoryCache.Set(
            key: key,
            value: value,
            absoluteExpirationRelativeToNow: expiry);
}