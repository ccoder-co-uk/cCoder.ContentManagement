// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Storages;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class PageRenderCacheServiceTests
{
    [Fact]
    public async Task ShouldPerformPageRenderCacheCrudAsync()
    {
        // Given
        PageRenderCache stored = CreatePageRenderCache();
        Mock<IPageRenderCacheBroker> brokerMock = new();

        brokerMock
            .Setup(expression: broker => broker.GetAllPageRenderCaches())
            .Returns(value: new[] { stored }.AsQueryable());

        brokerMock
            .Setup(expression: broker => broker.AddPageRenderCacheAsync(
                newPageRenderCache: It.IsAny<PageRenderCache>()))
            .ReturnsAsync(valueFunction: (PageRenderCache cache) => cache);

        brokerMock
            .Setup(expression: broker => broker.UpdatePageRenderCacheAsync(
                updatedPageRenderCache: It.IsAny<PageRenderCache>()))
            .ReturnsAsync(valueFunction: (PageRenderCache cache) => cache);

        brokerMock
            .Setup(expression: broker => broker.DeletePageRenderCacheAsync(
                pageRenderCacheId: stored.Id))
            .Returns(value: ValueTask.CompletedTask);

        PageRenderCacheService service = new(broker: brokerMock.Object);

        // When
        PageRenderCache[] all = [.. service.GetAllPageRenderCaches()];

        PageRenderCache selected = service.GetPageRenderCache(
            pageRenderCacheId: stored.Id);

        PageRenderCache added = await service.AddPageRenderCacheAsync(
            newPageRenderCache: stored);

        PageRenderCache updated = await service.UpdatePageRenderCacheAsync(
            updatedPageRenderCache: stored);

        await service.DeletePageRenderCacheAsync(
            pageRenderCacheId: stored.Id);

        // Then
        all.Should()
            .ContainSingle()
            .Which.Should()
            .BeSameAs(expected: stored);

        selected.Should()
            .BeSameAs(expected: stored);

        added.Should()
            .NotBeSameAs(unexpected: stored);

        added.Should()
            .BeEquivalentTo(expectation: stored);

        updated.Should()
            .NotBeSameAs(unexpected: stored);

        updated.Should()
            .BeEquivalentTo(expectation: stored);

        brokerMock.Verify(
            expression: broker => broker.DeletePageRenderCacheAsync(
                pageRenderCacheId: stored.Id),
            times: Times.Once());
    }

    private static PageRenderCache CreatePageRenderCache() =>
        new()
        {
            Id = "1_2_en-gb_default",
            AppId = 1,
            PageId = 2,
            Culture = "en-gb",
            Theme = "default",
            Path = "cached",
            Header = "header",
            Body = "body",
            SourceFingerprint = new string(c: 'A', count: 64),
            RenderedOn = DateTimeOffset.UtcNow
        };
}