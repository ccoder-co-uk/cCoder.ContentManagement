// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Exposures;

public interface IPageRenderCacheManager
{
    IQueryable<PageRenderCache> GetAll();

    PageRenderCache Get(string pageRenderCacheId);

    ValueTask<PageRenderCache> AddAsync(PageRenderCache newPageRenderCache);

    ValueTask<PageRenderCache> UpdateAsync(PageRenderCache updatedPageRenderCache);

    ValueTask DeleteAsync(string pageRenderCacheId);

    ValueTask DeleteAppAsync(int appId);

    ValueTask DeletePageAsync(int pageId);

}