// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class ContentOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultWhenGet()
    {
        // Given
        int id = 1;
        Content entity = CreateRandomContent();

        contentProcessingServiceMock.Setup(expression: x => x.GetContent(contentId: id))
            .Returns(value: entity);

        // When
        Content result = orchestrationService.GetContent(contentId: id);

        // Then

        result.Should()
            .BeEquivalentTo(expectation: entity);

        contentProcessingServiceMock.Verify(expression: x => x.GetContent(contentId: id), times: Times.Once);
        contentProcessingServiceMock.VerifyNoOtherCalls();
        contentEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}