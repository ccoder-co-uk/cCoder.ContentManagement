// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Exposures;
using cCoder.ContentManagement.Services.Aggregations;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Exposures;

public sealed partial class TemplateManagerTests
{
    [Fact]
    public async Task ReadContentAsync_WhenCalled_ShouldDelegateToTemplateOrchestrationAsync()
    {
        // Given
        const string expectedContent = "template-content";
        using MemoryStream source = new();
        Mock<ITemplateManagerAggregationService> aggregationServiceMock = new();

        aggregationServiceMock.Setup(
            expression: service => service.ReadContentAsync(source: source))
            .ReturnsAsync(value: expectedContent);

        TemplateManager templateManager = new(
            templateManagerAggregationService: aggregationServiceMock.Object);

        // When
        string actualContent = await templateManager.ReadContentAsync(source: source);

        // Then
        actualContent.Should()
            .Be(expected: expectedContent);

        aggregationServiceMock.VerifyAll();
    }

    [Fact]
    public void ConvertHtmlToPdf_WhenCalled_ShouldDelegateToTemplateOrchestration()
    {
        // Given
        const string html = "<html></html>";
        byte[] expectedPdf = [1, 2, 3];
        Mock<ITemplateManagerAggregationService> aggregationServiceMock = new();

        aggregationServiceMock.Setup(
            expression: service => service.ConvertHtmlToPdf(html: html))
            .Returns(value: expectedPdf);

        TemplateManager templateManager = new(
            templateManagerAggregationService: aggregationServiceMock.Object);

        // When
        byte[] actualPdf = templateManager.ConvertHtmlToPdf(html: html);

        // Then
        actualPdf.Should()
            .Equal(expected: expectedPdf);

        aggregationServiceMock.VerifyAll();
    }
}