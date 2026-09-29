// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class PageOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultWhenGetRoot()
    {
        // Given
        Page expected = CreateRandomPage();

        pageProcessingServiceMock.Setup(expression: x => x.GetRootPage(pageId: 1))
            .Returns(value: expected);

        // When
        Page result = orchestrationService.GetRootPage(pageId: 1);

        // Then
        result.Should()
            .BeEquivalentTo(expectation: expected);

        pageProcessingServiceMock.Verify(expression: x => x.GetRootPage(pageId: 1), times: Times.Once);
        pageProcessingServiceMock.VerifyNoOtherCalls();
        pageEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}