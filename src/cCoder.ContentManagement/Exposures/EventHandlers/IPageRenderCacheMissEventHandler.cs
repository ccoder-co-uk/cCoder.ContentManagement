// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Exposures.EventHandlers;

internal interface IPageRenderCacheMissEventHandler
{
    ValueTask RebuildMissingPageAsync(PageRenderCacheMiss cacheMiss);
}