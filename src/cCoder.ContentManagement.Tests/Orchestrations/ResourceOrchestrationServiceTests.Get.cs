// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class ResourceOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultWhenGet()
    {
        // Given
        int id = 1;
        Resource entity = CreateRandomResource();

        resourceProcessingServiceMock.Setup(expression: x => x.GetResource(resourceId: id))
            .Returns(value: entity);

        // When
        Resource result = orchestrationService.GetResource(resourceId: id);

        // Then

        result.Should()
            .BeEquivalentTo(expectation: entity);

        resourceProcessingServiceMock.Verify(expression: x => x.GetResource(resourceId: id), times: Times.Once);
        resourceProcessingServiceMock.VerifyNoOtherCalls();
        resourceEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}