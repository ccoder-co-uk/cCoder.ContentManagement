// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Processings;

public sealed partial class PageRenderCacheQueryProcessingServiceTests
{
    [Theory]
    [InlineData("en-gb-north", "en-gb")]
    [InlineData("en-us", "en")]
    [InlineData("fr", "")]
    [InlineData("", "")]
    public void ShouldReturnNearestAvailableCulture(
        string requestedCulture,
        string expectedCulture)
    {
        // Given
        PageRenderCache[] caches =
        [
            CreateCache(culture: string.Empty),
            CreateCache(culture: "en"),
            CreateCache(culture: "en-gb")
        ];

        Mock<IPageRenderCacheService> cacheService = new();

        cacheService.Setup(expression: service =>
                service.GetAllPageRenderCaches())
            .Returns(value: caches.AsQueryable());

        PageRenderCacheQueryProcessingService service = new(
            pageRenderCacheService: cacheService.Object);

        // When
        PageRenderCache result = service.GetPageRenderCache(
            pageId: 7,
            culture: requestedCulture,
            theme: "default");

        // Then
        result.Culture
            .Should()
            .Be(expected: expectedCulture);
    }

    private static PageRenderCache CreateCache(string culture) =>
        new()
        {
            Id = $"1_7_{culture}_default",
            AppId = 1,
            PageId = 7,
            Culture = culture,
            Theme = "default"
        };
}