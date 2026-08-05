// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Aggregations;

namespace cCoder.ContentManagement.Exposures.EventHandlers;

internal sealed class PageRenderCacheMissEventHandler(
    IPageRenderCacheBuildAggregationService pageRenderCacheBuildAggregationService)
        : IPageRenderCacheMissEventHandler
{
    public ValueTask RebuildMissingPageAsync(PageRenderCacheMiss cacheMiss) =>
        pageRenderCacheBuildAggregationService.BuildPageAsync(
            pageId: cacheMiss.PageId);
}