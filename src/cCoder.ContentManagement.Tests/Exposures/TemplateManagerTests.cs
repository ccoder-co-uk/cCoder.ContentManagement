// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Exposures;
using cCoder.ContentManagement.Services.Orchestrations;
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
        Mock<ITemplateOrchestrationService> templateOrchestrationServiceMock = new();

        templateOrchestrationServiceMock.Setup(
            expression: service => service.ReadContentAsync(source: source))
            .ReturnsAsync(value: expectedContent);

        TemplateManager templateManager = new(
            templateOrchestrationService: templateOrchestrationServiceMock.Object);

        // When
        string actualContent = await templateManager.ReadContentAsync(source: source);

        // Then
        actualContent.Should()
            .Be(expected: expectedContent);

        templateOrchestrationServiceMock.VerifyAll();
    }

    [Fact]
    public void ConvertHtmlToPdf_WhenCalled_ShouldDelegateToTemplateOrchestration()
    {
        // Given
        const string html = "<html></html>";
        byte[] expectedPdf = [1, 2, 3];
        Mock<ITemplateOrchestrationService> templateOrchestrationServiceMock = new();

        templateOrchestrationServiceMock.Setup(
            expression: service => service.ConvertHtmlToPdf(html: html))
            .Returns(value: expectedPdf);

        TemplateManager templateManager = new(
            templateOrchestrationService: templateOrchestrationServiceMock.Object);

        // When
        byte[] actualPdf = templateManager.ConvertHtmlToPdf(html: html);

        // Then
        actualPdf.Should()
            .Equal(expected: expectedPdf);

        templateOrchestrationServiceMock.VerifyAll();
    }
}