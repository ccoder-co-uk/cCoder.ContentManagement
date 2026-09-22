// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Aggregations;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Services.Orchestrations.Caching;
using cCoder.ContentManagement.Services.Orchestrations.PageContexts;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Aggregations;

public sealed partial class RenderAggregationServiceTests
{
    [Fact]
    public async Task RenderComponentRenderResultAsync_WhenCacheHasExpired_ShouldEnsureCommonObjectsAsync()
    {
        // Given
        HttpPageRenderContext context = new()
        {
            AppId = 17,
            Culture = "en-GB",
            Theme = "default"
        };

        ComponentRenderResult expected = new();

        Mock<ICommonObjectCacheOrchestrationService> cacheService =
            new(behavior: MockBehavior.Strict);

        Mock<IPageContextOrchestrationService> contextService = new();
        Mock<IComponentRenderOrchestrationService> componentService = new();

        cacheService.Setup(
            expression: service => service.EnsureAvailable());

        contextService
            .Setup(expression: service =>
                service.ResolvePageRenderContextAsync())
            .Returns(value: ValueTask.FromResult(result: context));

        componentService
            .Setup(expression: service => service.RenderComponentRenderResult(
                appId: 17,
                name: "Hero",
                culture: "en-GB",
                theme: "default"))
            .Returns(value: expected);

        RenderAggregationService service = new(
            pageContextOrchestrationService: contextService.Object,
            cachedPageRenderOrchestrationService:
                Mock.Of<ICachedPageRenderOrchestrationService>(),
            renderEventOrchestrationService:
                Mock.Of<IRenderEventOrchestrationService>(),
            renderDataOrchestrationService:
                Mock.Of<IRenderDataOrchestrationService>(),
            templateRenderOrchestrationService:
                Mock.Of<ITemplateRenderOrchestrationService>(),
            componentRenderOrchestrationService: componentService.Object,
            commonObjectCacheOrchestrationService: cacheService.Object);

        // When
        RenderResult actual = await service
            .RenderComponentRenderResultAsync(name: "Hero");

        // Then
        actual
            .Should()
            .BeSameAs(expected: expected);

        cacheService.Verify(
            expression: service => service.EnsureAvailable(),
            times: Times.Once);
    }
}