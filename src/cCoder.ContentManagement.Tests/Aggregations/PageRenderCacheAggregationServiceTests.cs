// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;
using cCoder.ContentManagement.Exposures.Caching;
using cCoder.ContentManagement.Services.Aggregations;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Aggregations;

public sealed partial class PageRenderCacheAggregationServiceTests
{
    [Fact]
    public async Task ShouldDelegatePageRenderCacheCrudAsync()
    {
        // Given
        Mock<IPageRenderCacheOrchestrationService> cacheService = new();
        PageRenderCache cache = new() { Id = "1_2__default" };
        IQueryable<PageRenderCache> caches = new[] { cache }.AsQueryable();

        cacheService.Setup(expression: service =>
            service.GetAllPageRenderCaches())
            .Returns(value: caches);

        cacheService.Setup(expression: service =>
            service.GetPageRenderCache(pageRenderCacheId: cache.Id))
            .Returns(value: cache);

        cacheService.Setup(expression: service =>
            service.AddPageRenderCacheAsync(newPageRenderCache: cache))
            .ReturnsAsync(value: cache);

        cacheService.Setup(expression: service =>
            service.UpdatePageRenderCacheAsync(updatedPageRenderCache: cache))
            .ReturnsAsync(value: cache);

        cacheService.Setup(expression: service =>
            service.DeletePageRenderCacheAsync(pageRenderCacheId: cache.Id))
            .Returns(value: ValueTask.CompletedTask);

        PageRenderCacheAggregationService service = CreateService(
            cacheService: cacheService);

        // When
        _ = service.GetAllPageRenderCaches();
        _ = service.GetPageRenderCache(pageRenderCacheId: cache.Id);
        _ = await service.AddPageRenderCacheAsync(newPageRenderCache: cache);
        _ = await service.UpdatePageRenderCacheAsync(updatedPageRenderCache: cache);
        await service.DeletePageRenderCacheAsync(pageRenderCacheId: cache.Id);

        // Then
        cacheService.VerifyAll();
    }

    [Fact]
    public async Task ShouldDelegateDirectAndEventInvalidationAsync()
    {
        // Given
        const int appId = 7;
        const int pageId = 11;
        Mock<IPageRenderCacheOrchestrationService> cacheService = new();

        cacheService.Setup(expression: service =>
            service.DeleteAppPageRenderCachesAsync(appId: appId))
            .Returns(value: ValueTask.CompletedTask);

        cacheService.Setup(expression: service =>
            service.DeleteAppPageRenderCachesFromEventAsync(appId: appId))
            .Returns(value: ValueTask.CompletedTask);

        cacheService.Setup(expression: service =>
            service.DeletePagePageRenderCachesAsync(pageId: pageId))
            .Returns(value: ValueTask.CompletedTask);

        cacheService.Setup(expression: service =>
            service.DeletePagePageRenderCachesFromEventAsync(pageId: pageId))
            .Returns(value: ValueTask.CompletedTask);

        PageRenderCacheAggregationService service = CreateService(
            cacheService: cacheService);

        // When
        await service.DeleteAppAsync(appId: appId);
        await service.DeleteAppAsync(appId: appId, fromEvent: true);
        await service.DeletePageAsync(pageId: pageId);
        await service.DeletePageAsync(pageId: pageId, fromEvent: true);

        // Then
        cacheService.VerifyAll();
    }

    [Fact]
    public async Task ShouldInvalidateAllCachedAppsForCommonRenderObjectsAsync()
    {
        // Given
        Mock<IPageRenderCacheOrchestrationService> cacheService = new();

        cacheService.Setup(expression: service =>
            service.GetAllPageRenderCaches())
            .Returns(value: new[]
            {
                new PageRenderCache { AppId = 7 },
                new PageRenderCache { AppId = 9 }
            }.AsQueryable());

        cacheService.Setup(expression: service =>
            service.DeleteAppPageRenderCachesFromEventAsync(appId: 7))
            .Returns(value: ValueTask.CompletedTask);

        cacheService.Setup(expression: service =>
            service.DeleteAppPageRenderCachesFromEventAsync(appId: 9))
            .Returns(value: ValueTask.CompletedTask);

        PageRenderCacheAggregationService service = CreateService(
            cacheService: cacheService);

        // When
        await service.InvalidateCommonObjectConsumersAsync(
            commonObjectType: "ContentManagement/Component",
            fromEvent: true);

        // Then
        cacheService.VerifyAll();
    }

    [Fact]
    public async Task ShouldRefreshCommonCacheBeforeInvalidatingImportedAppAsync()
    {
        // Given
        const int appId = 23;
        MockSequence sequence = new();
        Mock<ICommonObjectCache> commonObjectCache = new();
        Mock<IPageRenderCacheOrchestrationService> cacheService = new();

        commonObjectCache.InSequence(sequence: sequence)
            .Setup(expression: cache => cache.Refresh());

        cacheService.InSequence(sequence: sequence)
            .Setup(expression: service =>
                service.DeleteAppPageRenderCachesFromEventAsync(appId: appId))
            .Returns(value: ValueTask.CompletedTask);

        PageRenderCacheAggregationService service = CreateService(
            cacheService: cacheService,
            commonObjectCache: commonObjectCache);

        // When
        await service.RefreshCommonCacheAndInvalidateAppAsync(appId: appId);

        // Then
        commonObjectCache.VerifyAll();
        cacheService.VerifyAll();
    }

    private static PageRenderCacheAggregationService CreateService(
        Mock<IPageRenderCacheOrchestrationService> cacheService = null,
        Mock<ICommonObjectCache> commonObjectCache = null) =>
        new(
            pageRenderCacheOrchestrationService:
                (cacheService ??
                    new Mock<IPageRenderCacheOrchestrationService>()).Object,
            pageRenderCacheImportState: new PageRenderCacheImportState(),
            commonObjectCache:
                (commonObjectCache ?? new Mock<ICommonObjectCache>()).Object);
}