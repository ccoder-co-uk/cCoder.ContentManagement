// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class PageOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultWhenMenuFor()
    {
        // Given
        pageProcessingServiceMock.Setup(expression: x => x.MenuFor(pageId: 1, culture: "en-GB"))
            .Returns(value: "menu");

        // When
        string result = orchestrationService.MenuFor(pageId: 1, culture: "en-GB");

        // Then
        result.Should()
            .Be(expected: "menu");

        pageProcessingServiceMock.Verify(expression: x => x.MenuFor(pageId: 1, culture: "en-GB"), times: Times.Once);
        pageProcessingServiceMock.VerifyNoOtherCalls();
        pageEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}