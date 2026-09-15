// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Services.Processings;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Orchestrations;

public sealed partial class TemplateContentOrchestrationServiceTests
{
    [Fact]
    public async Task ReadContentAsync_WhenCalled_ShouldUseOnlyTemplateContentProcessingAsync()
    {
        // Given
        using MemoryStream source = new();
        const string expectedContent = "template-content";
        Mock<ITemplateContentProcessingService> contentMock = new(behavior: MockBehavior.Strict);
        Mock<IHtmlToPdfProcessingService> pdfMock = new(behavior: MockBehavior.Strict);

        contentMock.Setup(expression: service => service.ReadContentAsync(source: source))
            .ReturnsAsync(value: expectedContent);

        TemplateContentOrchestrationService orchestrationService = new(
            templateContentProcessingService: contentMock.Object,
            htmlToPdfProcessingService: pdfMock.Object);

        // When
        string actualContent = await orchestrationService.ReadContentAsync(source: source);

        // Then
        actualContent
            .Should()
            .Be(expected: expectedContent);

        contentMock.VerifyAll();
        pdfMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ConvertHtmlToPdf_WhenCalled_ShouldUseOnlyHtmlToPdfProcessing()
    {
        // Given
        const string html = "<html></html>";
        byte[] expectedPdf = [1, 2, 3];
        Mock<ITemplateContentProcessingService> contentMock = new(behavior: MockBehavior.Strict);
        Mock<IHtmlToPdfProcessingService> pdfMock = new(behavior: MockBehavior.Strict);

        pdfMock.Setup(expression: service => service.ConvertHtmlToPdf(html: html))
            .Returns(value: expectedPdf);

        TemplateContentOrchestrationService orchestrationService = new(
            templateContentProcessingService: contentMock.Object,
            htmlToPdfProcessingService: pdfMock.Object);

        // When
        byte[] actualPdf = orchestrationService.ConvertHtmlToPdf(html: html);

        // Then
        actualPdf
            .Should()
            .Equal(expected: expectedPdf);

        pdfMock.VerifyAll();
        contentMock.VerifyNoOtherCalls();
    }
}