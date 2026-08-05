// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Exposures.EventHandlers;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Aggregations;
using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Exposures.EventHandlers;

public partial class PageRenderCacheEventHandlersTests
{
    [Fact]
    public async Task ShouldCacheUncachedPageRenderAsync()
    {
        // Given
        Mock<IPageRenderCacheAggregationService> aggregationService = new();

        UncachedPageRenderEvent pageRenderEvent = new()
        {
            PageId = 17
        };

        aggregationService.Setup(expression: item =>
            item.RebuildPageAsync(
                pageId: pageRenderEvent.PageId,
                fromEvent: true))
            .ReturnsAsync(value: []);

        UncachedPageRenderEventHandler handlers = new(
            pageRenderCacheAggregationService: aggregationService.Object);

        // When
        await handlers.CachePageAsync(pageRenderEvent: pageRenderEvent);

        // Then
        aggregationService.VerifyAll();
    }

    [Fact]
    public async Task ShouldRebuildPageOnPageChangeAsync()
    {
        // Given
        Mock<IPageRenderCacheAggregationService> aggregationService = new();
        Page page = new() { Id = 17 };

        aggregationService.Setup(expression: item =>
            item.RebuildPageAsync(pageId: page.Id, fromEvent: true))
            .ReturnsAsync(value: []);

        PageRenderCacheEventHandlers handlers = new(
            pageRenderCacheAggregationService: aggregationService.Object);

        // When
        await handlers.RebuildPageAsync(page: page);

        // Then
        aggregationService.Verify(
            expression: item => item.RebuildPageAsync(
                pageId: page.Id,
                fromEvent: true),
            times: Times.Once());
    }

    [Fact]
    public async Task ShouldDeletePageAndAppCacheOnDeleteAsync()
    {
        // Given
        Mock<IPageRenderCacheAggregationService> aggregationService = new();
        Page deletedPage = new() { Id = 17 };
        App deletedApp = new() { Id = 23 };

        aggregationService.Setup(expression: item =>
            item.DeletePageAsync(pageId: deletedPage.Id, fromEvent: true))
            .Returns(value: ValueTask.CompletedTask);

        aggregationService.Setup(expression: item =>
            item.DeleteAppAsync(appId: deletedApp.Id, fromEvent: true))
            .Returns(value: ValueTask.CompletedTask);

        PageRenderCacheEventHandlers handlers = new(
            pageRenderCacheAggregationService: aggregationService.Object);

        // When
        await handlers.DeletePageAsync(deletedPage: deletedPage);
        await handlers.DeleteAppAsync(deletedApp: deletedApp);

        // Then
        aggregationService.VerifyAll();
    }

    [Fact]
    public async Task ShouldRebuildOwningAppOnComponentChangeAsync()
    {
        // Given
        Mock<IPageRenderCacheAggregationService> aggregationService = new();
        Component component = new() { AppId = 23 };

        aggregationService.Setup(expression: item =>
            item.RebuildAppAsync(appId: component.AppId, fromEvent: true))
            .ReturnsAsync(value: []);

        PageRenderCacheEventHandlers handlers = new(
            pageRenderCacheAggregationService: aggregationService.Object);

        // When
        await handlers.RebuildAppAsync(component: component);

        // Then
        aggregationService.VerifyAll();
    }

    [Fact]
    public async Task ShouldRebuildOwningAppForEveryAppScopedRenderDependencyAsync()
    {
        // Given
        const int appId = 23;
        Mock<IPageRenderCacheAggregationService> aggregationService = new();

        aggregationService.Setup(expression: item =>
            item.RebuildAppAsync(appId: appId, fromEvent: true))
            .ReturnsAsync(value: []);

        PageRenderCacheEventHandlers handlers = new(
            pageRenderCacheAggregationService: aggregationService.Object);

        // When
        await handlers.RebuildAppAsync(
            appCulture: new AppCulture { AppId = appId });

        await handlers.RebuildAppAsync(
            layout: new Layout { AppId = appId });

        await handlers.RebuildAppAsync(
            template: new Template { AppId = appId });

        await handlers.RebuildAppAsync(
            component: new Component { AppId = appId });

        await handlers.RebuildAppAsync(
            resource: new Resource { AppId = appId });

        await handlers.RebuildAppAsync(
            script: new Script { AppId = appId });

        // Then
        aggregationService.Verify(
            expression: item => item.RebuildAppAsync(
                appId: appId,
                fromEvent: true),
            times: Times.Exactly(callCount: 6));
    }

    [Fact]
    public async Task ShouldRebuildOwningPageForEveryPageScopedRenderDependencyAsync()
    {
        // Given
        const int pageId = 17;
        Mock<IPageRenderCacheAggregationService> aggregationService = new();

        aggregationService.Setup(expression: item =>
            item.RebuildPageAsync(pageId: pageId, fromEvent: true))
            .ReturnsAsync(value: []);

        PageRenderCacheEventHandlers handlers = new(
            pageRenderCacheAggregationService: aggregationService.Object);

        // When
        await handlers.RebuildPageAsync(
            content: new Content { PageId = pageId });

        await handlers.RebuildPageAsync(
            pageInfo: new PageInfo { PageId = pageId });

        // Then
        aggregationService.Verify(
            expression: item => item.RebuildPageAsync(
                pageId: pageId,
                fromEvent: true),
            times: Times.Exactly(callCount: 2));
    }

    [Fact]
    public async Task ShouldDelegateCommonCacheIdentityChangeAsync()
    {
        // Given
        Mock<IPageRenderCacheAggregationService> aggregationService = new();
        CommonObject commonObject = new() { Type = "Component" };

        aggregationService.Setup(expression: item =>
            item.RebuildCommonObjectConsumersAsync(
                commonObjectType: commonObject.Type,
                fromEvent: true))
            .ReturnsAsync(value: []);

        PageRenderCacheEventHandlers handlers = new(
            pageRenderCacheAggregationService: aggregationService.Object);

        // When
        await handlers.RebuildCommonCacheConsumersAsync(
            commonObject: commonObject);

        // Then
        aggregationService.VerifyAll();
    }
}