// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Exposures;

internal sealed class PageRenderCacheManager(
    IPageRenderCacheOrchestrationService pageRenderCacheOrchestrationService)
        : IPageRenderCacheManager
{
    public IQueryable<PageRenderCache> GetAll() =>
        pageRenderCacheOrchestrationService.GetAllPageRenderCaches();

    public PageRenderCache Get(string pageRenderCacheId) =>
        pageRenderCacheOrchestrationService.GetPageRenderCache(
            pageRenderCacheId: pageRenderCacheId);

    public ValueTask<PageRenderCache> AddAsync(
        PageRenderCache newPageRenderCache) =>
        pageRenderCacheOrchestrationService.AddPageRenderCacheAsync(
            newPageRenderCache: newPageRenderCache);

    public ValueTask<PageRenderCache> UpdateAsync(
        PageRenderCache updatedPageRenderCache) =>
        pageRenderCacheOrchestrationService.UpdatePageRenderCacheAsync(
            updatedPageRenderCache: updatedPageRenderCache);

    public ValueTask DeleteAsync(string pageRenderCacheId) =>
        pageRenderCacheOrchestrationService.DeletePageRenderCacheAsync(
            pageRenderCacheId: pageRenderCacheId);

    public ValueTask DeleteAppAsync(int appId) =>
        pageRenderCacheOrchestrationService.DeleteAppPageRenderCachesAsync(
            appId: appId);

    public ValueTask DeletePageAsync(int pageId) =>
        pageRenderCacheOrchestrationService.DeletePagePageRenderCachesAsync(
            pageId: pageId);

}