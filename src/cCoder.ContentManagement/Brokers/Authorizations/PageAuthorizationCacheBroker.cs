// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading;
using cCoder.CodeAnalysis.Exposures;
using cCoder.ContentManagement.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace cCoder.ContentManagement.Brokers.Authorizations;

internal sealed class PageAuthorizationCacheBroker(
    IMemoryCache memoryCache)
        : IPageAuthorizationCacheBroker, IUtilityBroker
{
    private CancellationTokenSource cacheCancellation = new();

    public PageAuthorizationData Get(string key) =>
        memoryCache.Get<PageAuthorizationData>(key: key);

    public void Set(
        string key,
        PageAuthorizationData pageAuthorizationData)
    {
        MemoryCacheEntryOptions options = new()
        {
            AbsoluteExpirationRelativeToNow =
                TimeSpan.FromSeconds(seconds: 30)
        };

        options.AddExpirationToken(
            expirationToken: new CancellationChangeToken(
                cancellationToken: cacheCancellation.Token));

        memoryCache.Set(
            key: key,
            value: pageAuthorizationData,
            options: options);
    }

    public void Clear()
    {
        CancellationTokenSource previous = Interlocked.Exchange(
            location1: ref cacheCancellation,
            value: new CancellationTokenSource());

        previous.Cancel();
        previous.Dispose();
    }
}