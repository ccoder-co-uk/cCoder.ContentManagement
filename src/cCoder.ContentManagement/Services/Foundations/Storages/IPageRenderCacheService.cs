// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal interface IPageRenderCacheService
{
    IQueryable<PageRenderCache> GetAllPageRenderCaches();

    PageRenderCache GetPageRenderCache(int pageRenderCacheId);

    ValueTask<PageRenderCache> AddPageRenderCacheAsync(PageRenderCache newPageRenderCache);

    ValueTask<PageRenderCache> UpdatePageRenderCacheAsync(PageRenderCache updatedPageRenderCache);

    ValueTask DeletePageRenderCacheAsync(int pageRenderCacheId);

    ValueTask ReplacePageRenderCachesAsync(int appId, int[] pageIds, PageRenderCache[] replacements);
}