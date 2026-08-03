// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Aggregations;

namespace cCoder.ContentManagement.Exposures.EventHandlers;

internal sealed class PageRenderCacheEventHandlers(
    IPageRenderAggregationService pageRenderAggregationService)
        : IPageRenderCacheEventHandlers
{
    public async ValueTask RebuildPageAsync(Page page) =>
        _ = await pageRenderAggregationService.RebuildPagePageRenderOperationAsync(
            operation: new PageRenderOperation { PageId = page.Id, RebuildCache = true });

    public ValueTask DeletePageAsync(Page deletedPage) =>
        pageRenderAggregationService.DeletePagePageRenderCacheFromEventAsync(pageId: deletedPage.Id);

    public async ValueTask RebuildAppAsync(App app) =>
        _ = await ExecuteRebuildAppAsync(appId: app.Id);

    public async ValueTask RebuildAppAsync(int appId) =>
        _ = await ExecuteRebuildAppAsync(appId: appId);

    public ValueTask DeleteAppAsync(App deletedApp) =>
        pageRenderAggregationService.DeleteAppPageRenderCacheFromEventAsync(appId: deletedApp.Id);

    public async ValueTask RebuildAppAsync(AppCulture appCulture) =>
        _ = await ExecuteRebuildAppAsync(appId: appCulture.AppId);

    public async ValueTask RebuildAppAsync(Layout layout) =>
        _ = await ExecuteRebuildAppAsync(appId: layout.AppId);

    public async ValueTask RebuildAppAsync(Template template) =>
        _ = await ExecuteRebuildAppAsync(appId: template.AppId);

    public async ValueTask RebuildAppAsync(Component component) =>
        _ = await ExecuteRebuildAppAsync(appId: component.AppId);

    public async ValueTask RebuildAppAsync(Resource resource) =>
        _ = await ExecuteRebuildAppAsync(appId: resource.AppId);

    public async ValueTask RebuildAppAsync(Script script) =>
        _ = await ExecuteRebuildAppAsync(appId: script.AppId);

    public async ValueTask RebuildPageAsync(Content content) =>
        _ = await pageRenderAggregationService.RebuildPagePageRenderOperationAsync(
            operation: new PageRenderOperation { PageId = content.PageId, RebuildCache = true });

    public async ValueTask RebuildPageAsync(PageInfo pageInfo) =>
        _ = await pageRenderAggregationService.RebuildPagePageRenderOperationAsync(
            operation: new PageRenderOperation { PageId = pageInfo.PageId, RebuildCache = true });

    public async ValueTask RebuildCommonCacheConsumersAsync(CommonObject commonObject) =>
        _ = await pageRenderAggregationService.RebuildCommonObjectPageRenderOperationAsync(
            operation: new PageRenderOperation { CommonObject = commonObject, RebuildCache = true });

    public async ValueTask RebuildMissingPageAsync(PageRenderCacheMiss cacheMiss) =>
        _ = await pageRenderAggregationService
            .RebuildMissingPagePageRenderOperationAsync(
                operation: new PageRenderOperation
                {
                    AppId = cacheMiss.AppId,
                    PageId = cacheMiss.PageId,
                    Culture = cacheMiss.Culture,
                    Theme = cacheMiss.Theme,
                    RebuildCache = true
                });

    private ValueTask<PageRenderOperation> ExecuteRebuildAppAsync(int appId) =>
        pageRenderAggregationService.RebuildAppPageRenderOperationAsync(
            operation: new PageRenderOperation { AppId = appId, RebuildCache = true });
}