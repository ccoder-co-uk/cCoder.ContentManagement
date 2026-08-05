// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Aggregations;

namespace cCoder.ContentManagement.Exposures.EventHandlers;

internal sealed class UncachedPageRenderEventHandler(
    IPageRenderCacheAggregationService pageRenderCacheAggregationService)
        : IUncachedPageRenderEventHandler
{
    public ValueTask CachePageAsync(
        UncachedPageRenderEvent pageRenderEvent) =>
        CacheRenderedPageAsync(pageRenderEvent: pageRenderEvent);

    private async ValueTask CacheRenderedPageAsync(
        UncachedPageRenderEvent pageRenderEvent) =>
        _ = await pageRenderCacheAggregationService.RebuildPageAsync(
            pageId: pageRenderEvent.PageId,
            fromEvent: true);
}