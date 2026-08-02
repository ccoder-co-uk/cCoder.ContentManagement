// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Brokers.Storages;

internal interface IPageRenderCacheBroker
{
    IQueryable<PageRenderCache> GetAllPageRenderCaches();

    ValueTask<PageRenderCache> AddPageRenderCacheAsync(PageRenderCache newPageRenderCache);

    ValueTask<PageRenderCache> UpdatePageRenderCacheAsync(PageRenderCache updatedPageRenderCache);

    ValueTask DeletePageRenderCacheAsync(PageRenderCache deletedPageRenderCache);

    ValueTask ReplacePageRenderCachesByAppIdAsync(int appId, PageRenderCache[] replacements);

    ValueTask ReplacePageRenderCachesByPageIdsAsync(int appId, int[] pageIds, PageRenderCache[] replacements);
}