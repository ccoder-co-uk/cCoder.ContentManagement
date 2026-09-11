// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Exposures;
using cCoder.ContentManagement.Services.Orchestrations;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Exposures;

public sealed class TemplateManagerTests
{
    [Fact]
    public async Task ReadContentAsync_WhenCalled_ShouldDelegateToTemplateOrchestrationAsync()
    {
        const string expectedContent = "template-content";
        using MemoryStream source = new();
        Mock<ITemplateOrchestrationService> templateOrchestrationServiceMock = new();

        templateOrchestrationServiceMock.Setup(service => service.ReadContentAsync(source))
            .ReturnsAsync(expectedContent);

        TemplateManager templateManager = new(
            templateOrchestrationService: templateOrchestrationServiceMock.Object);

        string actualContent = await templateManager.ReadContentAsync(source: source);

        actualContent.Should().Be(expectedContent);
        templateOrchestrationServiceMock.VerifyAll();
    }

    [Fact]
    public void ConvertHtmlToPdf_WhenCalled_ShouldDelegateToTemplateOrchestration()
    {
        const string html = "<html></html>";
        byte[] expectedPdf = [1, 2, 3];
        Mock<ITemplateOrchestrationService> templateOrchestrationServiceMock = new();

        templateOrchestrationServiceMock.Setup(service => service.ConvertHtmlToPdf(html))
            .Returns(expectedPdf);

        TemplateManager templateManager = new(
            templateOrchestrationService: templateOrchestrationServiceMock.Object);

        byte[] actualPdf = templateManager.ConvertHtmlToPdf(html: html);

        actualPdf.Should().Equal(expectedPdf);
        templateOrchestrationServiceMock.VerifyAll();
    }
}