// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.Extensions.Caching.Memory;

namespace cCoder.ContentManagement.Dependencies;

internal sealed class MemoryCacheDependency()
    : MemoryCache(new MemoryCacheOptions())
{
    protected override void Dispose(bool disposing) =>
        base.Dispose(disposing: disposing);
}