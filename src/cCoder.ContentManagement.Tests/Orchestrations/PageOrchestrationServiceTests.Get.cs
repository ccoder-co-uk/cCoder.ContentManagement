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
    public void ShouldReturnProcessingResultWhenGet()
    {
        // Given
        int id = 1;
        Page entity = CreateRandomPage();

        pageProcessingServiceMock.Setup(expression: x => x.GetPage(pageId: id))
            .Returns(value: entity);

        // When
        Page result = orchestrationService.GetPage(pageId: id);

        // Then

        result.Should()
            .BeEquivalentTo(expectation: entity);

        pageProcessingServiceMock.Verify(expression: x => x.GetPage(pageId: id), times: Times.Once);
        pageProcessingServiceMock.VerifyNoOtherCalls();
        pageEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}