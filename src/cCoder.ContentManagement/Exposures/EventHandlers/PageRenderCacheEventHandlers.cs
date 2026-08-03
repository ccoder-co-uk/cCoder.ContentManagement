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
            operation: new PageRenderOperation
            {
                PageId = page.Id,
                RebuildCache = true
            });

    public ValueTask DeletePageAsync(Page deletedPage) =>
        pageRenderAggregationService.DeletePagePageRenderCacheFromEventAsync(
            pageId: deletedPage.Id);

    public async ValueTask RebuildAppAsync(App app) =>
        _ = await RebuildAppAsync(appId: app.Id);

    public ValueTask DeleteAppAsync(App deletedApp) =>
        pageRenderAggregationService.DeleteAppPageRenderCacheFromEventAsync(
            appId: deletedApp.Id);

    public async ValueTask RebuildAppAsync(AppCulture appCulture) =>
        _ = await RebuildAppAsync(appId: appCulture.AppId);

    public async ValueTask RebuildAppAsync(Layout layout) =>
        _ = await RebuildAppAsync(appId: layout.AppId);

    public async ValueTask RebuildAppAsync(Template template) =>
        _ = await RebuildAppAsync(appId: template.AppId);

    public async ValueTask RebuildAppAsync(Component component) =>
        _ = await RebuildAppAsync(appId: component.AppId);

    public async ValueTask RebuildAppAsync(Resource resource) =>
        _ = await RebuildAppAsync(appId: resource.AppId);

    public async ValueTask RebuildAppAsync(Script script) =>
        _ = await RebuildAppAsync(appId: script.AppId);

    public async ValueTask RebuildPageAsync(Content content) =>
        _ = await pageRenderAggregationService.RebuildPagePageRenderOperationAsync(
            operation: new PageRenderOperation
            {
                PageId = content.PageId,
                RebuildCache = true
            });

    public async ValueTask RebuildPageAsync(PageInfo pageInfo) =>
        _ = await pageRenderAggregationService.RebuildPagePageRenderOperationAsync(
            operation: new PageRenderOperation
            {
                PageId = pageInfo.PageId,
                RebuildCache = true
            });

    public async ValueTask RebuildCommonCacheConsumersAsync(CommonObject commonObject) =>
        _ = await pageRenderAggregationService.RebuildCommonObjectPageRenderOperationAsync(
            operation: new PageRenderOperation
            {
                CommonObject = commonObject,
                RebuildCache = true
            });

    private ValueTask<PageRenderOperation> RebuildAppAsync(int appId) =>
        pageRenderAggregationService.RebuildAppPageRenderOperationAsync(
            operation: new PageRenderOperation
            {
                AppId = appId,
                RebuildCache = true
            });
}