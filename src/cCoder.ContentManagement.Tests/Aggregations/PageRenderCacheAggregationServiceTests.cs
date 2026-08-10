// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
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

        PageRenderCacheAggregationService service =
            CreateService(cacheService: cacheService);

        // When
        IQueryable<PageRenderCache> actualCaches =
            service.GetAllPageRenderCaches();

        PageRenderCache actualCache = service.GetPageRenderCache(
            pageRenderCacheId: cache.Id);

        PageRenderCache addedCache = await service.AddPageRenderCacheAsync(
            newPageRenderCache: cache);

        PageRenderCache updatedCache = await service.UpdatePageRenderCacheAsync(
            updatedPageRenderCache: cache);

        await service.DeletePageRenderCacheAsync(
            pageRenderCacheId: cache.Id);

        // Then
        Assert.Same(expected: caches, actual: actualCaches);
        Assert.Same(expected: cache, actual: actualCache);
        Assert.Same(expected: cache, actual: addedCache);
        Assert.Same(expected: cache, actual: updatedCache);
        cacheService.VerifyAll();
    }

    [Fact]
    public async Task ShouldCoalesceRepeatedCacheMissAfterPageWasCachedAsync()
    {
        // Given
        const int pageId = 11;

        PageRenderCache existingCache = new()
        {
            Id = "7_11__default",
            AppId = 7,
            PageId = pageId
        };

        Mock<IPageOrchestrationService> pageService = new();
        Mock<IPageRenderOrchestrationService> renderService = new();
        Mock<IPageRenderCacheOrchestrationService> cacheService = new();

        cacheService.Setup(expression: service =>
            service.GetAllPageRenderCaches())
            .Returns(value: new[] { existingCache }
                .AsQueryable());

        PageRenderCacheAggregationService service = CreateService(
            pageService: pageService,
            renderService: renderService,
            cacheService: cacheService);

        // When
        PageRenderCache[] caches = await service.CachePageAsync(
            pageId: pageId);

        // Then
        Assert.Same(
            expected: existingCache,
            actual: Assert.Single(collection: caches));

        pageService.VerifyNoOtherCalls();
        renderService.VerifyNoOtherCalls();
        cacheService.VerifyAll();
    }

    [Fact]
    public async Task ShouldDelegateDirectAndEventDeletesAsync()
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

        PageRenderCacheAggregationService service =
            CreateService(cacheService: cacheService);

        // When
        await service.DeleteAppAsync(appId: appId);
        await service.DeleteAppAsync(appId: appId, fromEvent: true);
        await service.DeletePageAsync(pageId: pageId);
        await service.DeletePageAsync(pageId: pageId, fromEvent: true);

        // Then
        cacheService.VerifyAll();
    }

    [Fact]
    public async Task ShouldBuildAndStoreMissingPageRenderCacheAsync()
    {
        // Given
        const int appId = 7;
        const int pageId = 11;

        App app = new()
        {
            Id = appId,
            DefaultTheme = "Default"
        };

        Page page = new()
        {
            Id = pageId,
            App = app
        };

        PageRenderResult result = new()
        {
            Path = "/Documentation",
            Title = "Documentation",
            HeaderHtml = "<head />",
            BodyHtml = "<main />"
        };

        Mock<IPageOrchestrationService> pageService = new();
        Mock<IPageRenderOrchestrationService> renderService = new();
        Mock<IPageRenderCacheOrchestrationService> cacheService = new();

        pageService.Setup(expression: service =>
            service.GetPageForRenderAsync(pageId: pageId))
            .ReturnsAsync(value: page);

        renderService.Setup(expression: service =>
            service.ProcessPageRenderOperation(
                operation: It.IsAny<PageRenderOperation>()))
            .Returns(valueFunction: (PageRenderOperation operation) =>
            {
                operation.Page = result;
                return operation;
            });

        cacheService.Setup(expression: service =>
            service.ReplacePageRenderCachesFromEventAsync(
                appId: appId,
                pageIds: It.Is<int[]>(match: pageIds =>
                    pageIds.SequenceEqual(second: new[] { pageId })),
                replacements: It.Is<PageRenderCache[]>(match: replacements =>
                    replacements.Length == 1 &&
                    replacements[0].Id == "7_11__default")))
            .Returns(value: ValueTask.CompletedTask);

        PageRenderCacheAggregationService service = CreateService(
            pageService: pageService,
            renderService: renderService,
            cacheService: cacheService);

        // When
        PageRenderCache[] caches = await service.CachePageAsync(
            pageId: pageId);

        // Then
        PageRenderCache cache = Assert.Single(collection: caches);
        Assert.Equal(expected: "7_11__default", actual: cache.Id);
        Assert.Equal(expected: result.HeaderHtml, actual: cache.Header);
        Assert.Equal(expected: result.BodyHtml, actual: cache.Body);

        Assert.False(
            condition: string.IsNullOrWhiteSpace(
                value: cache.SourceFingerprint));

        pageService.VerifyAll();
        renderService.VerifyAll();
        cacheService.VerifyAll();
    }

    [Fact]
    public async Task ShouldIgnoreUnsupportedCommonCacheObjectAsync()
    {
        // Given
        PageRenderCacheAggregationService service = CreateService();

        // When
        PageRenderCache[] caches =
            await service.RebuildCommonObjectConsumersAsync(
                commonObjectType: "Folder",
                fromEvent: true);

        // Then
        Assert.Empty(collection: caches);
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
        Mock<IAppOrchestrationService> appService = null,
        Mock<IPageOrchestrationService> pageService = null,
        Mock<IPageRenderOrchestrationService> renderService = null,
        Mock<IPageRenderCacheOrchestrationService> cacheService = null,
        Mock<ICommonObjectCache> commonObjectCache = null) =>
        new(
            appOrchestrationService:
                (appService ?? new Mock<IAppOrchestrationService>()).Object,
            pageOrchestrationService:
                (pageService ?? new Mock<IPageOrchestrationService>()).Object,
            pageRenderOrchestrationService:
                (renderService ?? new Mock<IPageRenderOrchestrationService>()).Object,
            pageRenderCacheOrchestrationService:
                (cacheService ??
                    new Mock<IPageRenderCacheOrchestrationService>()).Object,
            pageRenderCacheImportState: new PageRenderCacheImportState(),
            commonObjectCache:
                (commonObjectCache ?? new Mock<ICommonObjectCache>()).Object);
}