// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Models.PageRendering;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Orchestrations;

public partial class CachedPageRenderOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldReturnCachedPageWithoutRaisingMissEventAsync()
    {
        // Given
        Page page = CreatePage();
        User user = CreateUser();
        PageRenderCache cache = CreateCache();

        pageRenderCacheProcessingServiceMock
            .Setup(expression: processing => processing.GetPageRenderCache(
                pageRenderCacheId: cache.Id))
            .Returns(value: cache);

        // When
        CachedPageRenderOperation result = await service
            .RenderCachedPageRenderOperationAsync(
                operation: CreateOperation(page: page, user: user));

        // Then
        result.Should()
            .NotBeNull();

        result.RenderResult.BodyHtml
            .Should()
            .Contain(expected: user.DisplayName);

        result.RenderResult.HeaderHtml
            .Should()
            .Be(expected: cache.Header);

        eventProcessingServiceMock.Verify(
            expression: processing => processing
                .RaisePageRenderCacheMissEventAsync(
                    cacheMiss: It.IsAny<PageRenderCacheMiss>()),
            times: Times.Never);
    }

    [Fact]
    public async Task ShouldRaiseMissAndReturnRebuiltPageAsync()
    {
        // Given
        Page page = CreatePage();
        User user = CreateUser();
        PageRenderCache cache = CreateCache();
        int queryCount = 0;

        pageRenderCacheProcessingServiceMock
            .Setup(expression: processing => processing.GetPageRenderCache(
                pageRenderCacheId: cache.Id))
            .Returns(valueFunction: () => ++queryCount == 1 ? null : cache);

        eventProcessingServiceMock
            .Setup(expression: processing => processing
                .RaisePageRenderCacheMissEventAsync(
                    cacheMiss: It.Is<PageRenderCacheMiss>(match: miss =>
                        miss.AppId == cache.AppId
                        && miss.PageId == cache.PageId
                        && miss.Culture == "en-gb"
                        && miss.Theme == "default")))
            .Returns(value: ValueTask.CompletedTask);

        // When
        CachedPageRenderOperation result = await service
            .RenderCachedPageRenderOperationAsync(
                operation: CreateOperation(page: page, user: user));

        // Then
        result.RenderResult
            .Should()
            .NotBeNull();

        queryCount.Should()
            .Be(expected: 2);

        eventProcessingServiceMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldReturnNullWhenPageRemainsMissingAfterRebuildAsync()
    {
        // Given
        Page page = CreatePage();
        User user = CreateUser();
        PageRenderCache cache = CreateCache();

        pageRenderCacheProcessingServiceMock
            .Setup(expression: processing => processing.GetPageRenderCache(
                pageRenderCacheId: cache.Id))
            .Returns(value: (PageRenderCache)null);

        eventProcessingServiceMock
            .Setup(expression: processing => processing
                .RaisePageRenderCacheMissEventAsync(
                    cacheMiss: It.IsAny<PageRenderCacheMiss>()))
            .Returns(value: ValueTask.CompletedTask);

        // When
        CachedPageRenderOperation result = await service
            .RenderCachedPageRenderOperationAsync(
                operation: CreateOperation(page: page, user: user));

        // Then
        result.RenderResult
            .Should()
            .BeNull();

        pageRenderCacheProcessingServiceMock.Verify(
            expression: processing => processing.GetPageRenderCache(
                pageRenderCacheId: cache.Id),
            times: Times.Exactly(callCount: 2));

        eventProcessingServiceMock.VerifyAll();
    }

    private static Page CreatePage() =>
        new()
        {
            Id = 17,
            AppId = 3,
            Layout = "Default",
            ResourceKey = "Default",
            App = new App
            {
                Id = 3,
                DefaultCultureId = string.Empty,
                Resources = []
            }
        };

    private static CachedPageRenderOperation CreateOperation(
        Page page,
        User user) =>
        new()
        {
            AppId = 3,
            PageId = 17,
            Page = page,
            Culture = "EN-gb",
            Theme = "DEFAULT",
            User = user
        };

    private static User CreateUser() =>
        new()
        {
            Id = "paul",
            DisplayName = "Paul Ward",
            Email = "paul.ward@ccoder.co.uk",
            DefaultCultureId = "en-gb"
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
            Body = $"<main>{PageRenderRuntimeTokens.DisplayName}</main>",
            SourceFingerprint = "fingerprint",
            RenderedOn = DateTimeOffset.UtcNow
        };
}