// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Orchestrations;

public partial class CachedPageRenderOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldReturnCachedPageWithoutLoadingPageGraphAsync()
    {
        // Given
        PageRenderCache cache = CreateCache();

        pageRenderCacheProcessingServiceMock
            .Setup(expression: processing => processing.GetPageRenderCache(
                pageId: cache.PageId,
                culture: cache.Culture,
                theme: cache.Theme))
            .Returns(value: cache);

        // When
        HttpPageRenderOperation result = await service
            .RenderHttpPageRenderOperationAsync(
                operation: CreateOperation());

        // Then
        result.Response.Page.BodyHtml
            .Should()
            .Be(expected: cache.Body);

        result.Response.Page.HeaderHtml
            .Should()
            .Be(expected: cache.Header);

        eventProcessingServiceMock.Verify(
            expression: processing => processing
                .RaisePageRenderCacheMissEventAsync(
                    cacheMiss: It.IsAny<PageRenderCacheMiss>()),
            times: Times.Never);
    }

    [Fact]
    public async Task ShouldRaiseMissAndRequeryCacheAsync()
    {
        // Given
        PageRenderCache cache = CreateCache();
        int queryCount = 0;

        pageRenderCacheProcessingServiceMock
            .Setup(expression: processing => processing.GetPageRenderCache(
                pageId: cache.PageId,
                culture: cache.Culture,
                theme: cache.Theme))
            .Returns(valueFunction: () => ++queryCount == 1 ? null : cache);

        eventProcessingServiceMock
            .Setup(expression: processing => processing
                .RaisePageRenderCacheMissEventAsync(
                    cacheMiss: It.Is<PageRenderCacheMiss>(match: miss =>
                        miss.PageId == cache.PageId)))
            .Returns(value: ValueTask.CompletedTask);

        // When
        HttpPageRenderOperation result = await service
            .RenderHttpPageRenderOperationAsync(
                operation: CreateOperation());

        // Then
        result.Response
            .Should()
            .NotBeNull();

        queryCount.Should()
            .Be(expected: 2);

        eventProcessingServiceMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldReturnOperationWithoutResponseWhenPageRemainsMissingAsync()
    {
        // Given
        PageRenderCache cache = CreateCache();

        pageRenderCacheProcessingServiceMock
            .Setup(expression: processing => processing.GetPageRenderCache(
                pageId: cache.PageId,
                culture: cache.Culture,
                theme: cache.Theme))
            .Returns(value: (PageRenderCache)null);

        eventProcessingServiceMock
            .Setup(expression: processing => processing
                .RaisePageRenderCacheMissEventAsync(
                    cacheMiss: It.IsAny<PageRenderCacheMiss>()))
            .Returns(value: ValueTask.CompletedTask);

        // When
        HttpPageRenderOperation result = await service
            .RenderHttpPageRenderOperationAsync(
                operation: CreateOperation());

        // Then
        result.Response
            .Should()
            .BeNull();

        eventProcessingServiceMock.VerifyAll();
    }

    private static HttpPageRenderOperation CreateOperation() =>
        new()
        {
            Context = new HttpPageRenderContext
            {
                PageId = 17,
                Culture = " EN-GB ",
                Theme = " Default "
            }
        };

    private static PageRenderCache CreateCache() =>
        new()
        {
            Id = "3_17_en-gb_default",
            AppId = 3,
            PageId = 17,
            Culture = "en-gb",
            Theme = "default",
            ParentId = 2,
            Path = "/Documentation",
            Title = "Documentation",
            Description = "Documentation landing page",
            Keywords = "documentation",
            ShowOnMenus = true,
            Header = "<header>Documentation</header>",
            Body = "<main>Documentation</main>",
            SourceFingerprint = "fingerprint",
            RenderedOn = DateTimeOffset.UtcNow
        };
}