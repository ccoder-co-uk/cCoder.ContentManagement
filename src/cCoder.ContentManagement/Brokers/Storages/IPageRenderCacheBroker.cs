// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Brokers.Storages;

internal interface IPageRenderCacheBroker
{
    IQueryable<PageRenderCache> GetAllPageRenderCaches();

    PageRenderCache GetPageRenderCache(string pageRenderCacheId);

    ValueTask<PageRenderCache> AddPageRenderCacheAsync(PageRenderCache newPageRenderCache);

    ValueTask<PageRenderCache> UpdatePageRenderCacheAsync(PageRenderCache updatedPageRenderCache);

    ValueTask<PageRenderCache> StorePageRenderCacheAsync(PageRenderCache pageRenderCache);

    ValueTask DeletePageRenderCacheAsync(string pageRenderCacheId);

    ValueTask ReplacePageRenderCachesByAppIdAsync(int appId, PageRenderCache[] replacements);

    ValueTask ReplacePageRenderCachesByPageIdsAsync(int appId, int[] pageIds, PageRenderCache[] replacements);
}