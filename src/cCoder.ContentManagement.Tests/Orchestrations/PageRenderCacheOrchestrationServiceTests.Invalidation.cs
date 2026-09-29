// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class PageRenderCacheOrchestrationServiceTests
{
    [Fact]
    public async Task CommonObjectConsumers_WhenTypeDoesNotRender_ShouldNotInvalidateAsync()
    {
        // Given

        // When
        await orchestrationService.InvalidateCommonObjectConsumersAsync(
            commonObjectType: "Culture");

        // Then
        processingServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CommonObjectConsumers_WhenTypeRenders_ShouldInvalidateEachAppAsync()
    {
        // Given
        PageRenderCache first = CreatePageRenderCache(appId: 3, pageId: 11);
        PageRenderCache second = CreatePageRenderCache(appId: 3, pageId: 12);
        IQueryable<PageRenderCache> caches = new[] { first, second }.AsQueryable();

        processingServiceMock
            .Setup(expression: service => service.GetAllPageRenderCaches())
            .Returns(value: caches);

        processingServiceMock
            .Setup(expression: service => service.ReplacePageRenderCachesFromEventAsync(
                appId: 3,
                pageIds: It.Is<int[]>(match: pageIds =>
                    pageIds.Length == 1
                    && (pageIds[0] == 11 || pageIds[0] == 12)),
                replacements: It.Is<PageRenderCache[]>(match: replacements =>
                    replacements.Length == 0)))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.InvalidateCommonObjectConsumersAsync(
            commonObjectType: "ContentManagement/Component");

        // Then
        processingServiceMock.Verify(
            expression: service => service.GetAllPageRenderCaches(),
            times: Times.Exactly(callCount: 2));

        processingServiceMock.Verify(
            expression: service => service.ReplacePageRenderCachesFromEventAsync(
                appId: 3,
                pageIds: It.IsAny<int[]>(),
                replacements: It.IsAny<PageRenderCache[]>()),
            times: Times.Exactly(callCount: 2));
    }

    [Fact]
    public async Task PackageImport_WhenAppIsSpecified_ShouldInvalidateOnlyThatAppAsync()
    {
        // Given
        PageRenderCache cache = CreatePageRenderCache(appId: 7, pageId: 19);

        processingServiceMock
            .Setup(expression: service => service.GetAllPageRenderCaches())
            .Returns(value: new[] { cache }.AsQueryable());

        processingServiceMock
            .Setup(expression: service => service.ReplacePageRenderCachesFromEventAsync(
                appId: 7,
                pageIds: It.Is<int[]>(match: pageIds =>
                    pageIds.Length == 1 && pageIds[0] == 19),
                replacements: It.Is<PageRenderCache[]>(match: replacements =>
                    replacements.Length == 0)))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.InvalidatePackageAsync(appId: 7);

        // Then
        processingServiceMock.VerifyAll();
    }
}