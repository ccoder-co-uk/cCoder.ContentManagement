// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Services.Aggregations;

namespace cCoder.ContentManagement.Exposures.EventHandlers;

internal sealed class PageRenderCacheEventHandlers(
    IPageRenderCacheAggregationService pageRenderCacheAggregationService)
        : IPageRenderCacheEventHandlers
{
    public async ValueTask RebuildPageAsync(Page page) =>
        _ = await pageRenderCacheAggregationService.RebuildPageAsync(
            pageId: page.Id,
            fromEvent: true);

    public ValueTask DeletePageAsync(Page deletedPage) =>
        pageRenderCacheAggregationService.DeletePageAsync(
            pageId: deletedPage.Id,
            fromEvent: true);

    public async ValueTask RebuildAppAsync(App app) =>
        _ = await ExecuteRebuildAppAsync(appId: app.Id);

    public async ValueTask RebuildAppAsync(int appId) =>
        _ = await ExecuteRebuildAppAsync(appId: appId);

    public ValueTask InvalidateAppAsync(int appId) =>
        pageRenderCacheAggregationService.DeleteAppAsync(
            appId: appId,
            fromEvent: true);

    public ValueTask DeleteAppAsync(App deletedApp) =>
        pageRenderCacheAggregationService.DeleteAppAsync(
            appId: deletedApp.Id,
            fromEvent: true);

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
        _ = await pageRenderCacheAggregationService.RebuildPageAsync(
            pageId: content.PageId,
            fromEvent: true);

    public async ValueTask RebuildPageAsync(PageInfo pageInfo) =>
        _ = await pageRenderCacheAggregationService.RebuildPageAsync(
            pageId: pageInfo.PageId,
            fromEvent: true);

    public async ValueTask RebuildCommonCacheConsumersAsync(CommonObject commonObject) =>
        _ = await pageRenderCacheAggregationService.RebuildCommonObjectConsumersAsync(
            commonObjectType: commonObject.Type,
            fromEvent: true);

    private ValueTask<PageRenderCache[]> ExecuteRebuildAppAsync(int appId) =>
        pageRenderCacheAggregationService.RebuildAppAsync(
            appId: appId,
            fromEvent: true);
}