// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
namespace cCoder.ContentManagement.Models.Caching;

internal sealed class CacheEntry
{
    public string Key { get; init; }

    public DateTime AddedOn { get; init; }

    public object Value { get; init; }
}