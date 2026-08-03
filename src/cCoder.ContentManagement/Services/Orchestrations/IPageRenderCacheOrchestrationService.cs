// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal interface IPageRenderCacheOrchestrationService
{
    IQueryable<PageRenderCache> GetAllPageRenderCaches();

    PageRenderCache GetPageRenderCache(string pageRenderCacheId);

    ValueTask<PageRenderCache> AddPageRenderCacheAsync(PageRenderCache newPageRenderCache);

    ValueTask<PageRenderCache> UpdatePageRenderCacheAsync(PageRenderCache updatedPageRenderCache);

    ValueTask DeletePageRenderCacheAsync(string pageRenderCacheId);

    ValueTask DeleteAppPageRenderCachesAsync(int appId);

    ValueTask DeleteAppPageRenderCachesFromEventAsync(int appId);

    ValueTask DeletePagePageRenderCachesAsync(int pageId);

    ValueTask DeletePagePageRenderCachesFromEventAsync(int pageId);

    ValueTask ReplacePageRenderCachesAsync(
        int appId,
        int[] pageIds,
        PageRenderCache[] replacements);

    ValueTask ReplacePageRenderCachesFromEventAsync(
        int appId,
        int[] pageIds,
        PageRenderCache[] replacements);
}