// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Exposures.EventHandlers;
using cCoder.ContentManagement.Services.Aggregations;
using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Exposures.EventHandlers;

public partial class PageRenderCacheEventHandlersTests
{
    [Fact]
    public async Task ShouldInvalidatePageOnPageChangeAsync()
    {
        // Given
        Mock<IPageRenderCacheAggregationService> aggregationService = new();
        Page page = new() { Id = 17 };

        aggregationService.Setup(expression: item =>
            item.DeletePageAsync(pageId: page.Id, fromEvent: true))
            .Returns(value: ValueTask.CompletedTask);

        PageRenderCacheEventHandlers handlers = CreateHandlers(
            aggregationService: aggregationService.Object);

        // When
        await handlers.InvalidatePageAsync(page: page);

        // Then
        aggregationService.Verify(
            expression: item => item.DeletePageAsync(
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

        PageRenderCacheEventHandlers handlers = CreateHandlers(
            aggregationService: aggregationService.Object);

        // When
        await handlers.DeletePageAsync(deletedPage: deletedPage);
        await handlers.DeleteAppAsync(deletedApp: deletedApp);

        // Then
        aggregationService.VerifyAll();
    }

    [Fact]
    public async Task ShouldInvalidateAppCacheAfterPackageImportAsync()
    {
        // Given
        const int appId = 23;
        Mock<IPageRenderCacheAggregationService> aggregationService = new();

        aggregationService.Setup(expression: item =>
            item.DeleteAppAsync(appId: appId, fromEvent: true))
            .Returns(value: ValueTask.CompletedTask);

        PageRenderCacheEventHandlers handlers = CreateHandlers(
            aggregationService: aggregationService.Object);

        // When
        await handlers.InvalidateAppAsync(appId: appId);

        // Then
        aggregationService.VerifyAll();
    }

    [Fact]
    public async Task ShouldInvalidateOwningAppOnComponentChangeAsync()
    {
        // Given
        Mock<IPageRenderCacheAggregationService> aggregationService = new();
        Component component = new() { AppId = 23 };

        aggregationService.Setup(expression: item =>
            item.DeleteAppAsync(appId: component.AppId, fromEvent: true))
            .Returns(value: ValueTask.CompletedTask);

        PageRenderCacheEventHandlers handlers = CreateHandlers(
            aggregationService: aggregationService.Object);

        // When
        await handlers.InvalidateAppAsync(component: component);

        // Then
        aggregationService.VerifyAll();
    }

    [Fact]
    public async Task ShouldInvalidateOwningAppForEveryAppScopedRenderDependencyAsync()
    {
        // Given
        const int appId = 23;
        Mock<IPageRenderCacheAggregationService> aggregationService = new();

        aggregationService.Setup(expression: item =>
            item.DeleteAppAsync(appId: appId, fromEvent: true))
            .Returns(value: ValueTask.CompletedTask);

        PageRenderCacheEventHandlers handlers = CreateHandlers(
            aggregationService: aggregationService.Object);

        // When
        await handlers.InvalidateAppAsync(
            appCulture: new AppCulture { AppId = appId });

        await handlers.InvalidateAppAsync(
            layout: new Layout { AppId = appId });

        await handlers.InvalidateAppAsync(
            template: new Template { AppId = appId });

        await handlers.InvalidateAppAsync(
            component: new Component { AppId = appId });

        await handlers.InvalidateAppAsync(
            resource: new Resource { AppId = appId });

        await handlers.InvalidateAppAsync(
            script: new Script { AppId = appId });

        // Then
        aggregationService.Verify(
            expression: item => item.DeleteAppAsync(
                appId: appId,
                fromEvent: true),
            times: Times.Exactly(callCount: 6));
    }

    [Fact]
    public async Task ShouldInvalidateOwningPageForEveryPageScopedRenderDependencyAsync()
    {
        // Given
        const int pageId = 17;
        Mock<IPageRenderCacheAggregationService> aggregationService = new();

        aggregationService.Setup(expression: item =>
            item.DeletePageAsync(pageId: pageId, fromEvent: true))
            .Returns(value: ValueTask.CompletedTask);

        PageRenderCacheEventHandlers handlers = CreateHandlers(
            aggregationService: aggregationService.Object);

        // When
        await handlers.InvalidatePageAsync(
            content: new Content { PageId = pageId });

        await handlers.InvalidatePageAsync(
            pageInfo: new PageInfo { PageId = pageId });

        // Then
        aggregationService.Verify(
            expression: item => item.DeletePageAsync(
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
            item.InvalidateCommonObjectConsumersAsync(
                commonObjectType: commonObject.Type,
                fromEvent: true))
            .Returns(value: ValueTask.CompletedTask);

        PageRenderCacheEventHandlers handlers = CreateHandlers(
            aggregationService: aggregationService.Object);

        // When
        await handlers.InvalidateCommonCacheConsumersAsync(
            commonObject: commonObject);

        // Then
        aggregationService.VerifyAll();
    }

    [Fact]
    public async Task ShouldRefreshCommonCacheBeforeInvalidatingImportedAppAsync()
    {
        // Given
        const int appId = 23;
        Mock<IPageRenderCacheAggregationService> aggregationService = new();

        aggregationService.Setup(expression: item =>
            item.RefreshCommonCacheAndInvalidateAppAsync(appId: appId))
            .Returns(value: ValueTask.CompletedTask);

        PageRenderCacheEventHandlers handlers = CreateHandlers(
            aggregationService: aggregationService.Object);

        // When
        await handlers.RefreshCommonCacheAndInvalidateAppAsync(appId: appId);

        // Then
        aggregationService.VerifyAll();
    }

    private static PageRenderCacheEventHandlers CreateHandlers(
        IPageRenderCacheAggregationService aggregationService) =>
        new(
            pageRenderCacheAggregationService: aggregationService);
}