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
    public async Task ShouldRebuildExplicitCacheMissAsync()
    {
        // Given
        Mock<IPageRenderCacheBuildAggregationService> aggregationService = new();

        PageRenderCacheMiss cacheMiss = new()
        {
            PageId = 17
        };

        aggregationService.Setup(expression: item =>
            item.BuildPageAsync(pageId: cacheMiss.PageId))
            .Returns(value: ValueTask.CompletedTask);

        PageRenderCacheMissEventHandler handlers = new(
            pageRenderCacheBuildAggregationService: aggregationService.Object);

        // When
        await handlers.RebuildMissingPageAsync(cacheMiss: cacheMiss);

        // Then
        aggregationService.VerifyAll();
    }

    [Fact]
    public async Task ShouldRebuildPageOnPageChangeAsync()
    {
        // Given
        Mock<IPageRenderAggregationService> aggregationService = new();
        Page page = new() { Id = 17 };

        aggregationService.Setup(expression: item =>
            item.RebuildPagePageRenderOperationAsync(
                operation: It.Is<PageRenderOperation>(match: operation =>
                    operation.PageId == page.Id)))
            .ReturnsAsync(value: new PageRenderOperation());

        PageRenderCacheEventHandlers handlers = new(
            pageRenderAggregationService: aggregationService.Object);

        // When
        await handlers.RebuildPageAsync(page: page);

        // Then
        aggregationService.Verify(
            expression: item => item.RebuildPagePageRenderOperationAsync(
                operation: It.Is<PageRenderOperation>(match: operation =>
                    operation.PageId == page.Id)),
            times: Times.Once());
    }

    [Fact]
    public async Task ShouldDeletePageAndAppCacheOnDeleteAsync()
    {
        // Given
        Mock<IPageRenderAggregationService> aggregationService = new();
        Page deletedPage = new() { Id = 17 };
        App deletedApp = new() { Id = 23 };

        aggregationService.Setup(expression: item =>
            item.DeletePagePageRenderCacheFromEventAsync(pageId: deletedPage.Id))
            .Returns(value: ValueTask.CompletedTask);

        aggregationService.Setup(expression: item =>
            item.DeleteAppPageRenderCacheFromEventAsync(appId: deletedApp.Id))
            .Returns(value: ValueTask.CompletedTask);

        PageRenderCacheEventHandlers handlers = new(
            pageRenderAggregationService: aggregationService.Object);

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
        Mock<IPageRenderAggregationService> aggregationService = new();
        Component component = new() { AppId = 23 };

        aggregationService.Setup(expression: item =>
            item.RebuildAppPageRenderOperationAsync(
                operation: It.Is<PageRenderOperation>(match: operation =>
                    operation.AppId == component.AppId)))
            .ReturnsAsync(value: new PageRenderOperation());

        PageRenderCacheEventHandlers handlers = new(
            pageRenderAggregationService: aggregationService.Object);

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
        Mock<IPageRenderAggregationService> aggregationService = new();

        aggregationService.Setup(expression: item =>
            item.RebuildAppPageRenderOperationAsync(
                operation: It.Is<PageRenderOperation>(match: operation =>
                    operation.AppId == appId)))
            .ReturnsAsync(value: new PageRenderOperation());

        PageRenderCacheEventHandlers handlers = new(
            pageRenderAggregationService: aggregationService.Object);

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
            expression: item => item.RebuildAppPageRenderOperationAsync(
                operation: It.Is<PageRenderOperation>(match: operation =>
                    operation.AppId == appId)),
            times: Times.Exactly(callCount: 6));
    }

    [Fact]
    public async Task ShouldRebuildOwningPageForEveryPageScopedRenderDependencyAsync()
    {
        // Given
        const int pageId = 17;
        Mock<IPageRenderAggregationService> aggregationService = new();

        aggregationService.Setup(expression: item =>
            item.RebuildPagePageRenderOperationAsync(
                operation: It.Is<PageRenderOperation>(match: operation =>
                    operation.PageId == pageId)))
            .ReturnsAsync(value: new PageRenderOperation());

        PageRenderCacheEventHandlers handlers = new(
            pageRenderAggregationService: aggregationService.Object);

        // When
        await handlers.RebuildPageAsync(
            content: new Content { PageId = pageId });

        await handlers.RebuildPageAsync(
            pageInfo: new PageInfo { PageId = pageId });

        // Then
        aggregationService.Verify(
            expression: item => item.RebuildPagePageRenderOperationAsync(
                operation: It.Is<PageRenderOperation>(match: operation =>
                    operation.PageId == pageId)),
            times: Times.Exactly(callCount: 2));
    }

    [Fact]
    public async Task ShouldDelegateCommonCacheIdentityChangeAsync()
    {
        // Given
        Mock<IPageRenderAggregationService> aggregationService = new();
        CommonObject commonObject = new() { Type = "Component" };

        aggregationService.Setup(expression: item =>
            item.RebuildCommonObjectPageRenderOperationAsync(
                operation: It.Is<PageRenderOperation>(match: operation =>
                    operation.CommonObject == commonObject)))
            .ReturnsAsync(value: new PageRenderOperation());

        PageRenderCacheEventHandlers handlers = new(
            pageRenderAggregationService: aggregationService.Object);

        // When
        await handlers.RebuildCommonCacheConsumersAsync(
            commonObject: commonObject);

        // Then
        aggregationService.VerifyAll();
    }
}