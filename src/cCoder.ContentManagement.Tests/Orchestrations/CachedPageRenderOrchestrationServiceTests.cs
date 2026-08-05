// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Orchestrations;

public sealed partial class CachedPageRenderOrchestrationServiceTests
{
    [Fact]
    public void ShouldRenderCacheHit()
    {
        // Given
        HttpPageRenderOperation operation = CreateOperation();
        PageRenderCache cache = new() { PageId = 17 };
        Mock<IPageRenderCacheQueryProcessingService> queryService = new();
        Mock<ICachedPageRenderProcessingService> renderService = new();

        queryService.Setup(expression: service => service.GetPageRenderCache(
            pageId: 17,
            culture: "en-gb",
            theme: "default"))
            .Returns(value: cache);

        renderService.Setup(expression: service =>
            service.RenderPageRenderCacheOperation(
                operation: It.Is<PageRenderCacheOperation>(match: item =>
                    item.Cache == cache &&
                    item.RenderOperation == operation)))
            .Returns(valueFunction: (PageRenderCacheOperation item) => item);

        CachedPageRenderOrchestrationService service = new(
            queryProcessingService: queryService.Object,
            renderProcessingService: renderService.Object);

        // When
        HttpPageRenderOperation result =
            service.RenderHttpPageRenderOperation(operation: operation);

        // Then
        Assert.Same(expected: operation, actual: result);
        queryService.VerifyAll();
        renderService.VerifyAll();
    }

    [Fact]
    public void ShouldReturnUnchangedOperationOnCacheMiss()
    {
        // Given
        HttpPageRenderOperation operation = CreateOperation();
        Mock<IPageRenderCacheQueryProcessingService> queryService = new();

        queryService.Setup(expression: service => service.GetPageRenderCache(
            pageId: 17,
            culture: "en-gb",
            theme: "default"))
            .Returns(value: (PageRenderCache)null);

        CachedPageRenderOrchestrationService service = new(
            queryProcessingService: queryService.Object,
            renderProcessingService:
                Mock.Of<ICachedPageRenderProcessingService>());

        // When
        HttpPageRenderOperation result =
            service.RenderHttpPageRenderOperation(operation: operation);

        // Then
        Assert.Same(expected: operation, actual: result);
        Assert.Null(@object: result.Response);
        queryService.VerifyAll();
    }

    private static HttpPageRenderOperation CreateOperation() =>
        new()
        {
            Context = new HttpPageRenderContext
            {
                PageId = 17,
                Culture = "en-GB",
                Theme = "Default"
            }
        };
}