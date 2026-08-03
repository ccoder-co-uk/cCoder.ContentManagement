// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal interface IPageRenderCacheProcessingService
{
    ValueTask<PageRenderCache> AddPageRenderCacheAsync(PageRenderCache newPageRenderCache);

    ValueTask<PageRenderCache> UpdatePageRenderCacheAsync(PageRenderCache updatedPageRenderCache);

    ValueTask DeletePageRenderCacheAsync(string pageRenderCacheId);

    ValueTask ReplacePageRenderCachesAsync(int appId, int[] pageIds, PageRenderCache[] replacements);

    ValueTask ReplacePageRenderCachesFromEventAsync(
        int appId,
        int[] pageIds,
        PageRenderCache[] replacements);
}