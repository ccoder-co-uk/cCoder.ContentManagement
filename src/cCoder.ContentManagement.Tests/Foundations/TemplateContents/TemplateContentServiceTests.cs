// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Services.Foundations.TemplateContents;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Foundations.TemplateContents;

public sealed partial class TemplateContentServiceTests
{
    [Fact]
    public async Task ReadContentAsync_WhenCalled_ShouldDelegateToTemplateStreamBrokerAsync()
    {
        // Given
        using Stream source = new MemoryStream();
        string expectedContent = "<html></html>";
        Mock<ITemplateStreamBroker> brokerMock = new(behavior: MockBehavior.Strict);

        brokerMock
            .Setup(expression: broker => broker.ReadAsync(source: source))
            .ReturnsAsync(value: expectedContent);

        TemplateContentService service = new(
            templateStreamBroker: brokerMock.Object);

        // When
        string actualContent = await service.ReadContentAsync(source: source);

        // Then
        actualContent
            .Should()
            .Be(expected: expectedContent);

        brokerMock.VerifyAll();
    }

}