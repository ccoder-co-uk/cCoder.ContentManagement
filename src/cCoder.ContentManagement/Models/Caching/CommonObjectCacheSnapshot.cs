// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections.Generic;
using cCoder.Data.Models;

namespace cCoder.ContentManagement.Models.Caching;

internal sealed class CommonObjectCacheSnapshot
{
    public IReadOnlyDictionary<string, object> Items { get; init; }

    public CommonObject[] LatestSet { get; init; }
}