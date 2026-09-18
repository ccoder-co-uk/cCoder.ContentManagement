// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Exposures;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Processings;

public sealed partial class PageRenderCacheProcessingServiceTests
{
    [Fact]
    public async Task ShouldStoreOnlyTheExactNormalizedVariantAsync()
    {
        // Given
        PageRenderCache cache = new()
        {
            AppId = 3,
            PageId = 17,
            Culture = " EN-GB ",
            Theme = " Dark ",
            Path = "Admin/AppManagement",
            Header = "header",
            Body = "body"
        };

        const string expectedId = "3_17_en-gb_dark";
        Mock<IPageRenderCacheService> cacheService = new();

        cacheService.Setup(expression: service =>
            service.StorePageRenderCacheAsync(
                pageRenderCache: cache))
            .ReturnsAsync(value: cache);

        PageRenderCacheProcessingService service = new(
            service: cacheService.Object);

        // When
        PageRenderCache result = await service.StorePageRenderCacheAsync(
            pageRenderCache: cache);

        // Then
        Assert.Same(expected: cache, actual: result);
        Assert.Equal(expected: expectedId, actual: cache.Id);
        Assert.Equal(expected: "en-gb", actual: cache.Culture);
        Assert.Equal(expected: "dark", actual: cache.Theme);
        cacheService.VerifyAll();
    }
}