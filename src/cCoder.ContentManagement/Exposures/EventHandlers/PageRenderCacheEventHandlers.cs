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
    public ValueTask InvalidatePageAsync(Page page) =>
        pageRenderCacheAggregationService.DeletePageAsync(
            pageId: page.Id,
            fromEvent: true);

    public ValueTask DeletePageAsync(Page deletedPage) =>
        pageRenderCacheAggregationService.DeletePageAsync(
            pageId: deletedPage.Id,
            fromEvent: true);

    public ValueTask InvalidateAppAsync(App app) =>
        InvalidateAppAsync(appId: app.Id);

    public ValueTask InvalidateAppAsync(int appId) =>
        pageRenderCacheAggregationService.DeleteAppAsync(
            appId: appId,
            fromEvent: true);

    public ValueTask DeleteAppAsync(App deletedApp) =>
        pageRenderCacheAggregationService.DeleteAppAsync(
            appId: deletedApp.Id,
            fromEvent: true);

    public ValueTask InvalidateAppAsync(AppCulture appCulture) =>
        InvalidateAppAsync(appId: appCulture.AppId);

    public ValueTask InvalidateAppAsync(Layout layout) =>
        InvalidateAppAsync(appId: layout.AppId);

    public ValueTask InvalidateAppAsync(Template template) =>
        InvalidateAppAsync(appId: template.AppId);

    public ValueTask InvalidateAppAsync(Component component) =>
        InvalidateAppAsync(appId: component.AppId);

    public ValueTask InvalidateAppAsync(Resource resource) =>
        InvalidateAppAsync(appId: resource.AppId);

    public ValueTask InvalidateAppAsync(Script script) =>
        InvalidateAppAsync(appId: script.AppId);

    public ValueTask InvalidatePageAsync(Content content) =>
        pageRenderCacheAggregationService.DeletePageAsync(
            pageId: content.PageId,
            fromEvent: true);

    public ValueTask InvalidatePageAsync(PageInfo pageInfo) =>
        pageRenderCacheAggregationService.DeletePageAsync(
            pageId: pageInfo.PageId,
            fromEvent: true);

    public ValueTask InvalidateCommonCacheConsumersAsync(CommonObject commonObject) =>
        pageRenderCacheAggregationService.InvalidateCommonObjectConsumersAsync(
            commonObjectType: commonObject.Type,
            fromEvent: true);
}