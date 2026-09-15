// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Foundations.TemplateContents;
using cCoder.ContentManagement.Services.Processings;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Processings;

public sealed partial class HtmlToPdfProcessingServiceTests
{
    [Fact]
    public void ConvertHtmlToPdf_WhenCalled_ShouldDelegateToHtmlToPdfFoundation()
    {
        // Given
        const string html = "<html></html>";
        byte[] expectedPdf = [1, 2, 3];
        Mock<IHtmlToPdfService> serviceMock = new(behavior: MockBehavior.Strict);

        serviceMock.Setup(expression: service => service.ConvertHtmlToPdf(html: html))
            .Returns(value: expectedPdf);

        HtmlToPdfProcessingService processingService = new(
            htmlToPdfService: serviceMock.Object);

        // When
        byte[] actualPdf = processingService.ConvertHtmlToPdf(html: html);

        // Then
        actualPdf
            .Should()
            .Equal(expected: expectedPdf);

        serviceMock.VerifyAll();
    }
}