// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Aggregations;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Services.Orchestrations.PageContexts;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Aggregations;

public sealed partial class RenderAggregationServiceTests
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

        PageRenderResponse expectedResponse = new()
        {
            Page = new PageRenderResult
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
                service.RenderHttpPageRenderOperation(
                    operation: It.Is<HttpPageRenderOperation>(match:
                        operation => operation.Context == context)))
            .Returns(valueFunction: (HttpPageRenderOperation operation) =>
            {
                operation.Response = expectedResponse;
                return operation;
            });

        RenderAggregationService service = new(
            pageContextOrchestrationService: contextService.Object,
            cachedPageRenderOrchestrationService: cachedService.Object,
            uncachedPageRenderOrchestrationService: uncachedService.Object,
            templateRenderOrchestrationService:
                Mock.Of<ITemplateRenderOrchestrationService>(),
            componentRenderOrchestrationService:
                Mock.Of<IComponentRenderOrchestrationService>());

        // When
        RenderResult result = await service
            .RenderPageRenderResultAsync();

        PageRenderResponse actualResponse = result.PageResponse;

        // Then
        actualResponse
            .Should()
            .BeSameAs(expected: expectedResponse);

        actualResponse.Page.HeaderHtml
            .Should()
            .Contain(expected: "nonce='request-nonce'");

        actualResponse.Page.BodyHtml
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

        PageRenderResponse expectedResponse = new()
        {
            Page = new PageRenderResult
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
                operation.Response = expectedResponse;
                return ValueTask.FromResult(result: operation);
            });

        RenderAggregationService service = new(
            pageContextOrchestrationService: contextService.Object,
            cachedPageRenderOrchestrationService: cachedService.Object,
            uncachedPageRenderOrchestrationService: uncachedService.Object,
            templateRenderOrchestrationService:
                Mock.Of<ITemplateRenderOrchestrationService>(),
            componentRenderOrchestrationService:
                Mock.Of<IComponentRenderOrchestrationService>());

        // When
        RenderResult result = await service
            .RenderPageRenderResultAsync();

        PageRenderResponse actualResponse = result.PageResponse;

        // Then
        actualResponse
            .Should()
            .BeSameAs(expected: expectedResponse);

        cachedService.VerifyNoOtherCalls();

        uncachedService.Verify(
            expression: item => item.RenderHttpPageRenderOperationAsync(
                operation: It.IsAny<HttpPageRenderOperation>()),
            times: Times.Once);
    }

    [Fact]
    public async Task ShouldRenderTemplateUsingResolvedRequestContextAsync()
    {
        // Given
        HttpPageRenderContext context = new()
        {
            AppId = 17,
            Culture = "en-GB"
        };

        TemplateRenderResult expected = new() { Content = "template" };
        Mock<IPageContextOrchestrationService> contextService = new();
        Mock<ITemplateRenderOrchestrationService> templateService = new();

        contextService.Setup(expression: service =>
                service.ResolvePageRenderContextAsync())
            .Returns(value: ValueTask.FromResult(result: context));

        templateService.Setup(expression: service =>
                service.RenderTemplateRenderResult(
                    appId: 17,
                    name: "Welcome",
                    culture: "en-gb",
                    model: It.IsAny<object>()))
            .Returns(value: expected);

        RenderAggregationService service = new(
            pageContextOrchestrationService: contextService.Object,
            cachedPageRenderOrchestrationService:
                Mock.Of<ICachedPageRenderOrchestrationService>(),
            uncachedPageRenderOrchestrationService:
                Mock.Of<IUncachedPageRenderOrchestrationService>(),
            templateRenderOrchestrationService: templateService.Object,
            componentRenderOrchestrationService:
                Mock.Of<IComponentRenderOrchestrationService>());

        // When
        RenderResult actual = await service
            .RenderTemplateRenderResultAsync(
                name: "Welcome",
                model: new { Name = "Paul" });

        // Then
        actual
            .Should()
            .BeSameAs(expected: expected);

        templateService.VerifyAll();
    }

    [Fact]
    public async Task ShouldRenderComponentUsingResolvedRequestContextAsync()
    {
        // Given
        HttpPageRenderContext context = new()
        {
            AppId = 17,
            Culture = "en-GB",
            Theme = "Default"
        };

        ComponentRenderResult expected = new() { Content = "component" };
        Mock<IPageContextOrchestrationService> contextService = new();
        Mock<IComponentRenderOrchestrationService> componentService = new();

        contextService.Setup(expression: service =>
                service.ResolvePageRenderContextAsync())
            .Returns(value: ValueTask.FromResult(result: context));

        componentService.Setup(expression: service =>
                service.RenderComponentRenderResult(
                    appId: 17,
                    name: "Hero",
                    culture: "en-gb",
                    theme: "default"))
            .Returns(value: expected);

        RenderAggregationService service = new(
            pageContextOrchestrationService: contextService.Object,
            cachedPageRenderOrchestrationService:
                Mock.Of<ICachedPageRenderOrchestrationService>(),
            uncachedPageRenderOrchestrationService:
                Mock.Of<IUncachedPageRenderOrchestrationService>(),
            templateRenderOrchestrationService:
                Mock.Of<ITemplateRenderOrchestrationService>(),
            componentRenderOrchestrationService: componentService.Object);

        // When
        RenderResult actual = await service
            .RenderComponentRenderResultAsync(name: "Hero");

        // Then
        actual
            .Should()
            .BeSameAs(expected: expected);

        componentService.VerifyAll();
    }
}