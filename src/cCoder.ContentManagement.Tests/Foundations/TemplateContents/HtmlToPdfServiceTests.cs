// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Services.Foundations.TemplateContents;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Foundations.TemplateContents;

public sealed partial class HtmlToPdfServiceTests
{
    [Fact]
    public void ConvertHtmlToPdf_WhenCalled_ShouldDelegateToHtmlToPdfBroker()
    {
        // Given
        string html = "<html></html>";
        byte[] expectedContent = [1, 2, 3];
        Mock<IHtmlToPdfBroker> brokerMock = new(behavior: MockBehavior.Strict);

        brokerMock
            .Setup(expression: broker => broker.ConvertHtmlToPdf(html: html))
            .Returns(value: expectedContent);

        HtmlToPdfService service = new(htmlToPdfBroker: brokerMock.Object);

        // When
        byte[] actualContent = service.ConvertHtmlToPdf(html: html);

        // Then
        actualContent
            .Should()
            .Equal(expected: expectedContent);

        brokerMock.VerifyAll();
    }
}