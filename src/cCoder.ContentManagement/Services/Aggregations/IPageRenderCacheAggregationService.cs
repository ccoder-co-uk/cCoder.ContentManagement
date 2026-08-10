// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Aggregations;

internal interface IPageRenderCacheAggregationService
{
    IQueryable<PageRenderCache> GetAllPageRenderCaches();

    PageRenderCache GetPageRenderCache(string pageRenderCacheId);

    ValueTask<PageRenderCache> AddPageRenderCacheAsync(
        PageRenderCache newPageRenderCache);

    ValueTask<PageRenderCache> UpdatePageRenderCacheAsync(
        PageRenderCache updatedPageRenderCache);

    ValueTask DeletePageRenderCacheAsync(string pageRenderCacheId);

    ValueTask DeleteAppAsync(int appId, bool fromEvent = false);

    ValueTask RefreshCommonCacheAndInvalidateAppAsync(int appId);

    ValueTask DeletePageAsync(int pageId, bool fromEvent = false);

    ValueTask InvalidateCommonObjectConsumersAsync(
        string commonObjectType,
        bool fromEvent = false);
}