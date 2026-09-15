// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Foundations.TemplateContents;
using cCoder.ContentManagement.Services.Processings;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Processings;

public sealed partial class TemplateContentProcessingServiceTests
{
    [Fact]
    public async Task ReadContentAsync_WhenCalled_ShouldDelegateToTemplateContentFoundationAsync()
    {
        // Given
        using MemoryStream source = new();
        const string expectedContent = "template-content";
        Mock<ITemplateContentService> serviceMock = new(behavior: MockBehavior.Strict);

        serviceMock.Setup(expression: service => service.ReadContentAsync(source: source))
            .ReturnsAsync(value: expectedContent);

        TemplateContentProcessingService processingService = new(
            templateContentService: serviceMock.Object);

        // When
        string actualContent = await processingService.ReadContentAsync(source: source);

        // Then
        actualContent
            .Should()
            .Be(expected: expectedContent);

        serviceMock.VerifyAll();
    }
}