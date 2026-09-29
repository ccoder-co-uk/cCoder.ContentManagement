// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class LayoutOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDelegateToProcessingServiceWhenDeleteAllAsync()
    {
        // Given
        Layout[] entities = [CreateRandomLayout()];

        layoutProcessingServiceMock.Setup(expression: x => x.GetLayout(layoutId: entities[0].Id))
            .Returns(value: entities[0]);

        layoutEventProcessingServiceMock.Setup(expression: x => x.RaiseLayoutDeleteEventAsync(entity: entities[0], userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        layoutProcessingServiceMock.Setup(expression: x => x.DeleteAsync(layoutId: entities[0].Id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAllLayoutAsync(deletedLayout: entities);

        // Then
        layoutProcessingServiceMock.Verify(expression: x => x.GetLayout(layoutId: entities[0].Id), times: Times.Once);
        layoutEventProcessingServiceMock.Verify(expression: x => x.RaiseLayoutDeleteEventAsync(entity: entities[0], userId: CurrentUserId), times: Times.Once);
        layoutProcessingServiceMock.Verify(expression: x => x.DeleteAsync(layoutId: entities[0].Id), times: Times.Once);
    }

}