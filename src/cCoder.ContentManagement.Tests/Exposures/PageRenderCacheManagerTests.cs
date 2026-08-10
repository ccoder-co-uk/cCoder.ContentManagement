// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Exposures;
using cCoder.ContentManagement.Services.Aggregations;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Exposures;

public sealed partial class PageRenderCacheManagerTests
{
    [Fact]
    public async Task ShouldDelegatePageRenderCacheOperationsAsync()
    {
        // Given
        const int appId = 3;
        const int pageId = 17;
        PageRenderCache cache = new() { Id = "3_17__default" };
        Mock<IPageRenderCacheAggregationService> aggregationService = new();

        aggregationService.Setup(expression: service =>
            service.GetAllPageRenderCaches())
            .Returns(value: new[] { cache }.AsQueryable());

        aggregationService.Setup(expression: service =>
            service.GetPageRenderCache(pageRenderCacheId: cache.Id))
            .Returns(value: cache);

        aggregationService.Setup(expression: service =>
            service.AddPageRenderCacheAsync(newPageRenderCache: cache))
            .ReturnsAsync(value: cache);

        aggregationService.Setup(expression: service =>
            service.UpdatePageRenderCacheAsync(updatedPageRenderCache: cache))
            .ReturnsAsync(value: cache);

        aggregationService.Setup(expression: service =>
            service.DeletePageRenderCacheAsync(pageRenderCacheId: cache.Id))
            .Returns(value: ValueTask.CompletedTask);

        aggregationService.Setup(expression: service =>
            service.DeleteAppAsync(appId: appId, fromEvent: false))
            .Returns(value: ValueTask.CompletedTask);

        aggregationService.Setup(expression: service =>
            service.DeletePageAsync(pageId: pageId, fromEvent: false))
            .Returns(value: ValueTask.CompletedTask);

        PageRenderCacheManager manager = new(
            pageRenderCacheAggregationService: aggregationService.Object);

        // When
        _ = manager.GetAll();
        _ = manager.Get(pageRenderCacheId: cache.Id);
        _ = await manager.AddAsync(newPageRenderCache: cache);
        _ = await manager.UpdateAsync(updatedPageRenderCache: cache);
        await manager.DeleteAsync(pageRenderCacheId: cache.Id);
        await manager.DeleteAppAsync(appId: appId);
        await manager.DeletePageAsync(pageId: pageId);
        // Then
        aggregationService.VerifyAll();
    }
}