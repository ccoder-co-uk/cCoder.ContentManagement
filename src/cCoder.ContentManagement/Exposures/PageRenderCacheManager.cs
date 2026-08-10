// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Aggregations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Exposures;

internal sealed class PageRenderCacheManager(
    IPageRenderCacheAggregationService pageRenderCacheAggregationService)
        : IPageRenderCacheManager
{
    public IQueryable<PageRenderCache> GetAll() =>
        pageRenderCacheAggregationService.GetAllPageRenderCaches();

    public PageRenderCache Get(string pageRenderCacheId) =>
        pageRenderCacheAggregationService.GetPageRenderCache(
            pageRenderCacheId: pageRenderCacheId);

    public ValueTask<PageRenderCache> AddAsync(
        PageRenderCache newPageRenderCache) =>
        pageRenderCacheAggregationService.AddPageRenderCacheAsync(
            newPageRenderCache: newPageRenderCache);

    public ValueTask<PageRenderCache> UpdateAsync(
        PageRenderCache updatedPageRenderCache) =>
        pageRenderCacheAggregationService.UpdatePageRenderCacheAsync(
            updatedPageRenderCache: updatedPageRenderCache);

    public ValueTask DeleteAsync(string pageRenderCacheId) =>
        pageRenderCacheAggregationService.DeletePageRenderCacheAsync(
            pageRenderCacheId: pageRenderCacheId);

    public ValueTask DeleteAppAsync(int appId) =>
        pageRenderCacheAggregationService.DeleteAppAsync(appId: appId);

    public ValueTask DeletePageAsync(int pageId) =>
        pageRenderCacheAggregationService.DeletePageAsync(pageId: pageId);

}