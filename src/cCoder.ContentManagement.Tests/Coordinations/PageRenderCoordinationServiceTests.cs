// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Coordinations;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Services.Orchestrations.PageContexts;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Coordinations;

public sealed partial class PageRenderCoordinationServiceTests
{
    [Fact]
    public async Task ShouldReturnCachedResponseWithoutRenderingUncachedAsync()
    {
        // Given
        HttpPageRenderContext context = new()
        {
            PageId = 7,
            Nonce = "request-nonce"
        };

        PageRenderResponse response = new()
        {
            Page = new RenderResult
            {
                HeaderHtml = "<style nonce='[request[nonce]]'></style>",
                BodyHtml = "<script nonce='[request[nonce]]'></script>"
            }
        };

        Mock<IPageContextOrchestrationService> contextService = new();
        Mock<ICachedPageRenderOrchestrationService> cachedService = new();
        Mock<IUncachedPageRenderOrchestrationService> uncachedService = new();

        contextService.Setup(expression: service =>
                service.ResolvePageRenderContextAsync())
            .Returns(value: ValueTask.FromResult(result: context));

        cachedService.Setup(expression: service =>
                service.RenderHttpPageRenderOperationAsync(
                    operation: It.Is<HttpPageRenderOperation>(match:
                        operation => operation.Context == context)))
            .Returns(valueFunction: (HttpPageRenderOperation operation) =>
            {
                operation.Response = response;
                return ValueTask.FromResult(result: operation);
            });

        PageRenderCoordinationService service = new(
            pageContextOrchestrationService: contextService.Object,
            cachedPageRenderOrchestrationService: cachedService.Object,
            uncachedPageRenderOrchestrationService: uncachedService.Object);

        // When
        PageRenderResponse result = await service
            .RenderPageRenderResponseAsync();

        // Then
        result
            .Should()
            .BeSameAs(expected: response);

        result.Page.HeaderHtml
            .Should()
            .Contain(expected: "nonce='request-nonce'");

        result.Page.BodyHtml
            .Should()
            .Contain(expected: "nonce='request-nonce'");

        uncachedService.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(true, 7)]
    [InlineData(false, null)]
    public async Task ShouldRenderUncachedOnlyOnceWhenCacheIsNotApplicableAsync(
        bool edit,
        int? pageId)
    {
        // Given
        HttpPageRenderContext context = new()
        {
            Edit = edit,
            PageId = pageId,
            Nonce = "request-nonce"
        };

        PageRenderResponse response = new()
        {
            Page = new RenderResult
            {
                HeaderHtml = string.Empty,
                BodyHtml = string.Empty
            }
        };

        Mock<IPageContextOrchestrationService> contextService = new();
        Mock<ICachedPageRenderOrchestrationService> cachedService = new();
        Mock<IUncachedPageRenderOrchestrationService> uncachedService = new();

        contextService.Setup(expression: service =>
                service.ResolvePageRenderContextAsync())
            .Returns(value: ValueTask.FromResult(result: context));

        uncachedService.Setup(expression: service =>
                service.RenderHttpPageRenderOperationAsync(
                    operation: It.IsAny<HttpPageRenderOperation>()))
            .Returns(valueFunction: (HttpPageRenderOperation operation) =>
            {
                operation.Response = response;
                return ValueTask.FromResult(result: operation);
            });

        PageRenderCoordinationService service = new(
            pageContextOrchestrationService: contextService.Object,
            cachedPageRenderOrchestrationService: cachedService.Object,
            uncachedPageRenderOrchestrationService: uncachedService.Object);

        // When
        PageRenderResponse result = await service
            .RenderPageRenderResponseAsync();

        // Then
        result
            .Should()
            .BeSameAs(expected: response);

        cachedService.VerifyNoOtherCalls();

        uncachedService.Verify(
            expression: item => item.RenderHttpPageRenderOperationAsync(
                operation: It.IsAny<HttpPageRenderOperation>()),
            times: Times.Once);
    }
}