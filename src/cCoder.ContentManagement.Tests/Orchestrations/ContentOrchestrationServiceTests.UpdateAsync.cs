// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class ContentOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseUpdateEventAsyncWhenUpdateAsync()
    {
        // Given
        Content entity = CreateRandomContent();

        contentProcessingServiceMock.Setup(expression: x => x.UpdateContentAsync(updatedContent: entity))
            .ReturnsAsync(value: entity);

        contentEventProcessingServiceMock
            .Setup(expression: x => x.RaiseContentUpdateEventAsync(entity: entity, userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        Content result = await orchestrationService.UpdateContentAsync(updatedContent: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        contentProcessingServiceMock.Verify(expression: x => x.UpdateContentAsync(updatedContent: entity), times: Times.Once);
        contentEventProcessingServiceMock.Verify(expression: x => x.RaiseContentUpdateEventAsync(entity: entity, userId: CurrentUserId), times: Times.Once);
    }

}