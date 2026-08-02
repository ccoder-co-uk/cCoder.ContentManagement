// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Aggregations;
using cCoder.ContentManagement.Services.Foundations.ServiceProviders;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models;

namespace cCoder.ContentManagement.Exposures;

internal sealed class PageRenderCacheManager(
    IServiceProviderExecutionService serviceProviderExecutionService)
        : IPageRenderCacheManager
{
    public IQueryable<PageRenderCache> GetAll() =>
        serviceProviderExecutionService.Execute<
            IPageRenderCacheOrchestrationService,
            IQueryable<PageRenderCache>>(
                name: "PageRenderCache",
                operation: service => service.GetAllPageRenderCaches());

    public PageRenderCache Get(int pageRenderCacheId) =>
        serviceProviderExecutionService.Execute<
            IPageRenderCacheOrchestrationService,
            PageRenderCache>(
                name: "PageRenderCache",
                operation: service => service.GetPageRenderCache(
                    pageRenderCacheId: pageRenderCacheId));

    public ValueTask<PageRenderCache> AddAsync(
        PageRenderCache newPageRenderCache) =>
        serviceProviderExecutionService.Execute<
            IPageRenderCacheOrchestrationService,
            ValueTask<PageRenderCache>>(
                name: "PageRenderCache",
                operation: service => service.AddPageRenderCacheAsync(
                    newPageRenderCache: newPageRenderCache));

    public ValueTask<PageRenderCache> UpdateAsync(
        PageRenderCache updatedPageRenderCache) =>
        serviceProviderExecutionService.Execute<
            IPageRenderCacheOrchestrationService,
            ValueTask<PageRenderCache>>(
                name: "PageRenderCache",
                operation: service => service.UpdatePageRenderCacheAsync(
                    updatedPageRenderCache: updatedPageRenderCache));

    public ValueTask DeleteAsync(int pageRenderCacheId) =>
        serviceProviderExecutionService.Execute<
            IPageRenderCacheOrchestrationService,
            ValueTask>(
                name: "PageRenderCache",
                operation: service => service.DeletePageRenderCacheAsync(
                    pageRenderCacheId: pageRenderCacheId));

    public ValueTask DeleteAppAsync(int appId) =>
        serviceProviderExecutionService.Execute<
            IPageRenderAggregationService,
            ValueTask>(
                name: "PageRender",
                operation: service => service.DeleteAppPageRenderCacheAsync(
                    appId: appId));

    public ValueTask DeletePageAsync(int pageId) =>
        serviceProviderExecutionService.Execute<
            IPageRenderAggregationService,
            ValueTask>(
                name: "PageRender",
                operation: service => service.DeletePagePageRenderCacheAsync(
                    pageId: pageId));

    public async ValueTask<PageRenderCache[]> RebuildAppAsync(int appId)
    {
        PageRenderOperation result = await serviceProviderExecutionService.Execute<
            IPageRenderAggregationService,
            ValueTask<PageRenderOperation>>(
                name: "PageRender",
                operation: service => service.RebuildAppPageRenderOperationAsync(
                    operation: new PageRenderOperation
                    {
                        AppId = appId
                    }));

        return result.PageRenderCaches;
    }

    public async ValueTask<PageRenderCache[]> RebuildPageAsync(int pageId)
    {
        PageRenderOperation result = await serviceProviderExecutionService.Execute<
            IPageRenderAggregationService,
            ValueTask<PageRenderOperation>>(
                name: "PageRender",
                operation: service => service.RebuildPagePageRenderOperationAsync(
                    operation: new PageRenderOperation
                    {
                        PageId = pageId
                    }));

        return result.PageRenderCaches;
    }

    public async ValueTask<PageRenderCache[]> RebuildAllAppsAsync()
    {
        PageRenderOperation result = await serviceProviderExecutionService.Execute<
            IPageRenderAggregationService,
            ValueTask<PageRenderOperation>>(
                name: "PageRender",
                operation: service => service.RebuildAllAppsPageRenderOperationAsync(
                    operation: new PageRenderOperation()));

        return result.PageRenderCaches;
    }

    public async ValueTask<PageRenderCache[]> RebuildCommonCacheConsumersAsync(
        CommonObject commonObject)
    {
        PageRenderOperation result = await serviceProviderExecutionService.Execute<
            IPageRenderAggregationService,
            ValueTask<PageRenderOperation>>(
                name: "PageRender",
                operation: service => service.RebuildCommonObjectPageRenderOperationAsync(
                    operation: new PageRenderOperation
                    {
                        CommonObject = commonObject
                    }));

        return result.PageRenderCaches;
    }
}