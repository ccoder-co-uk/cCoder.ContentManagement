// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class PageRenderCacheOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnAllPageRenderCaches()
    {
        // Given
        PageRenderCache cache = CreatePageRenderCache();
        IQueryable<PageRenderCache> caches = new[] { cache }.AsQueryable();

        queryProcessingServiceMock
            .Setup(expression: service => service.GetAllPageRenderCaches())
            .Returns(value: caches);

        // When
        IQueryable<PageRenderCache> result =
            orchestrationService.GetAllPageRenderCaches();

        // Then
        result.Should()
            .BeSameAs(expected: caches);
    }

    [Fact]
    public void ShouldReturnPageRenderCacheById()
    {
        // Given
        PageRenderCache cache = CreatePageRenderCache();

        queryProcessingServiceMock
            .Setup(expression: service => service.GetPageRenderCache(
                pageRenderCacheId: cache.Id))
            .Returns(value: cache);

        // When
        PageRenderCache result = orchestrationService.GetPageRenderCache(
            pageRenderCacheId: cache.Id);

        // Then
        result.Should()
            .BeSameAs(expected: cache);
    }

    [Fact]
    public async Task ShouldDelegateCrudOperationsAsync()
    {
        // Given
        PageRenderCache cache = CreatePageRenderCache();

        processingServiceMock
            .Setup(expression: service => service.AddPageRenderCacheAsync(
                newPageRenderCache: cache))
            .ReturnsAsync(value: cache);

        processingServiceMock
            .Setup(expression: service => service.UpdatePageRenderCacheAsync(
                updatedPageRenderCache: cache))
            .ReturnsAsync(value: cache);

        processingServiceMock
            .Setup(expression: service => service.DeletePageRenderCacheAsync(
                pageRenderCacheId: cache.Id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        PageRenderCache added = await orchestrationService
            .AddPageRenderCacheAsync(newPageRenderCache: cache);

        PageRenderCache updated = await orchestrationService
            .UpdatePageRenderCacheAsync(updatedPageRenderCache: cache);

        await orchestrationService.DeletePageRenderCacheAsync(
            pageRenderCacheId: cache.Id);

        // Then
        added.Should()
            .BeSameAs(expected: cache);

        updated.Should()
            .BeSameAs(expected: cache);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ShouldDeleteDistinctAppPagesAsync(bool fromEvent)
    {
        // Given
        const int appId = 10;
        int[] expectedPageIds = [20, 21];

        PageRenderCache[] caches =
        [
            CreatePageRenderCache(appId: appId, pageId: 20),
            CreatePageRenderCache(appId: appId, pageId: 20),
            CreatePageRenderCache(appId: appId, pageId: 21),
            CreatePageRenderCache(appId: 11, pageId: 22)
        ];

        queryProcessingServiceMock
            .Setup(expression: service => service.GetAllPageRenderCaches())
            .Returns(value: caches.AsQueryable());

        if (fromEvent)
        {
            foreach (int pageId in expectedPageIds)
            {
                processingServiceMock
                    .Setup(expression: service => service.ReplacePageRenderCachesFromEventAsync(
                        appId: appId,
                        pageIds: It.Is<int[]>(match: ids => ids.SequenceEqual(
                            second: new[] { pageId })),
                        replacements: It.Is<PageRenderCache[]>(
                            match: items => items.Length == 0)))
                    .Returns(value: ValueTask.CompletedTask);
            }
        }
        else
        {
            foreach (int pageId in expectedPageIds)
            {
                processingServiceMock
                    .Setup(expression: service => service.ReplacePageRenderCachesAsync(
                        appId: appId,
                        pageIds: It.Is<int[]>(match: ids => ids.SequenceEqual(
                            second: new[] { pageId })),
                        replacements: It.Is<PageRenderCache[]>(
                            match: items => items.Length == 0)))
                    .Returns(value: ValueTask.CompletedTask);
            }
        }

        // When
        if (fromEvent)
        {
            await orchestrationService.DeleteAppPageRenderCachesFromEventAsync(
                appId: appId);
        }
        else
        {
            await orchestrationService.DeleteAppPageRenderCachesAsync(
                appId: appId);
        }

        // Then
        processingServiceMock.VerifyAll();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ShouldDeletePageCacheUsingOwningAppAsync(bool fromEvent)
    {
        // Given
        PageRenderCache cache = CreatePageRenderCache(appId: 10, pageId: 20);

        queryProcessingServiceMock
            .Setup(expression: service => service.GetAllPageRenderCaches())
            .Returns(value: new[] { cache }.AsQueryable());

        if (fromEvent)
        {
            processingServiceMock
                .Setup(expression: service => service.ReplacePageRenderCachesFromEventAsync(
                    appId: cache.AppId,
                    pageIds: It.Is<int[]>(match: ids => ids.SequenceEqual(
                        second: new[] { cache.PageId })),
                    replacements: It.Is<PageRenderCache[]>(
                        match: items => items.Length == 0)))
                .Returns(value: ValueTask.CompletedTask);
        }
        else
        {
            processingServiceMock
                .Setup(expression: service => service.ReplacePageRenderCachesAsync(
                    appId: cache.AppId,
                    pageIds: It.Is<int[]>(match: ids => ids.SequenceEqual(
                        second: new[] { cache.PageId })),
                    replacements: It.Is<PageRenderCache[]>(
                        match: items => items.Length == 0)))
                .Returns(value: ValueTask.CompletedTask);
        }

        // When
        if (fromEvent)
        {
            await orchestrationService.DeletePagePageRenderCachesFromEventAsync(
                pageId: cache.PageId);
        }
        else
        {
            await orchestrationService.DeletePagePageRenderCachesAsync(
                pageId: cache.PageId);
        }

        // Then
        processingServiceMock.VerifyAll();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ShouldNotReplaceWhenDeletedPageIsNotCachedAsync(bool fromEvent)
    {
        // Given
        queryProcessingServiceMock
            .Setup(expression: service => service.GetAllPageRenderCaches())
            .Returns(value: Array.Empty<PageRenderCache>()
                .AsQueryable());

        // When
        if (fromEvent)
        {
            await orchestrationService.DeletePagePageRenderCachesFromEventAsync(
                pageId: 20);
        }
        else
        {
            await orchestrationService.DeletePagePageRenderCachesAsync(
                pageId: 20);
        }

        // Then
        processingServiceMock.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ShouldDelegateReplacementAsync(bool fromEvent)
    {
        // Given
        const int appId = 10;
        int[] pageIds = [20];

        PageRenderCache[] replacements =
        [
            CreatePageRenderCache(
                appId: appId,
                pageId: 20)
        ];

        if (fromEvent)
        {
            processingServiceMock
                .Setup(expression: service => service.ReplacePageRenderCachesFromEventAsync(
                    appId: appId,
                    pageIds: pageIds,
                    replacements: replacements))
                .Returns(value: ValueTask.CompletedTask);
        }
        else
        {
            processingServiceMock
                .Setup(expression: service => service.ReplacePageRenderCachesAsync(
                    appId: appId,
                    pageIds: pageIds,
                    replacements: replacements))
                .Returns(value: ValueTask.CompletedTask);
        }

        // When
        if (fromEvent)
        {
            await orchestrationService.ReplacePageRenderCachesFromEventAsync(
                appId: appId,
                pageIds: pageIds,
                replacements: replacements);
        }
        else
        {
            await orchestrationService.ReplacePageRenderCachesAsync(
                appId: appId,
                pageIds: pageIds,
                replacements: replacements);
        }

        // Then
        processingServiceMock.VerifyAll();
    }
}