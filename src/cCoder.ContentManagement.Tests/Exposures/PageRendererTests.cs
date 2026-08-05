// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Exposures;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Aggregations;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Exposures;

public sealed partial class PageRendererTests
{
    [Fact]
    public async Task ShouldReturnPageRenderResponseAsync()
    {
        // Given
        PageRenderResponse expected = new();
        Mock<IRenderAggregationService> renderAggregationService = new();

        renderAggregationService.Setup(expression: service =>
                service.RenderPageRenderResultAsync())
            .Returns(value: ValueTask.FromResult<RenderResult>(
                result: expected));

        PageRenderer renderer = new(
            renderAggregationService: renderAggregationService.Object);

        // When
        PageRenderResponse actual = await renderer.RenderAsync();

        // Then
        actual
            .Should()
            .BeSameAs(expected: expected);
    }
}