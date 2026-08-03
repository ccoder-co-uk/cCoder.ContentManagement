// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models;

namespace cCoder.ContentManagement.Exposures;

public interface IPageRenderCacheManager
{
    IQueryable<PageRenderCache> GetAll();

    PageRenderCache Get(int pageRenderCacheId);

    ValueTask<PageRenderCache> AddAsync(PageRenderCache newPageRenderCache);

    ValueTask<PageRenderCache> UpdateAsync(PageRenderCache updatedPageRenderCache);

    ValueTask DeleteAsync(int pageRenderCacheId);

    ValueTask DeleteAppAsync(int appId);

    ValueTask DeletePageAsync(int pageId);

    ValueTask<PageRenderCache[]> RebuildAppAsync(int appId);

    ValueTask<PageRenderCache[]> RebuildPageAsync(int pageId);

    ValueTask<PageRenderCache[]> RebuildAllAppsAsync();

    ValueTask<PageRenderCache[]> RebuildCommonCacheConsumersAsync(
        CommonObject commonObject);
}