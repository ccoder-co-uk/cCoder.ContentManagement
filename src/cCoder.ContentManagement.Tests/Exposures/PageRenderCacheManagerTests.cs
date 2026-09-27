// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Exposures;
using cCoder.ContentManagement.Services.Orchestrations;
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
        Mock<IPageRenderCacheOrchestrationService> orchestrationService = new();

        orchestrationService.Setup(expression: service =>
            service.GetAllPageRenderCaches())
            .Returns(value: new[] { cache }.AsQueryable());

        orchestrationService.Setup(expression: service =>
            service.GetPageRenderCache(pageRenderCacheId: cache.Id))
            .Returns(value: cache);

        orchestrationService.Setup(expression: service =>
            service.AddPageRenderCacheAsync(newPageRenderCache: cache))
            .ReturnsAsync(value: cache);

        orchestrationService.Setup(expression: service =>
            service.UpdatePageRenderCacheAsync(updatedPageRenderCache: cache))
            .ReturnsAsync(value: cache);

        orchestrationService.Setup(expression: service =>
            service.DeletePageRenderCacheAsync(pageRenderCacheId: cache.Id))
            .Returns(value: ValueTask.CompletedTask);

        orchestrationService.Setup(expression: service =>
            service.DeleteAppPageRenderCachesAsync(appId: appId))
            .Returns(value: ValueTask.CompletedTask);

        orchestrationService.Setup(expression: service =>
            service.DeletePagePageRenderCachesAsync(pageId: pageId))
            .Returns(value: ValueTask.CompletedTask);

        PageRenderCacheManager manager = new(
            pageRenderCacheOrchestrationService: orchestrationService.Object);

        // When
        _ = manager.GetAll();
        _ = manager.Get(pageRenderCacheId: cache.Id);
        _ = await manager.AddAsync(newPageRenderCache: cache);
        _ = await manager.UpdateAsync(updatedPageRenderCache: cache);
        await manager.DeleteAsync(pageRenderCacheId: cache.Id);
        await manager.DeleteAppAsync(appId: appId);
        await manager.DeletePageAsync(pageId: pageId);
        // Then
        orchestrationService.VerifyAll();
    }
}