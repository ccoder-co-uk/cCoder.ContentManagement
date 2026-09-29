// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class ResourceOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDelegateToProcessingServiceWhenDeleteAllAsync()
    {
        // Given
        Resource[] entities = [CreateRandomResource()];

        resourceProcessingServiceMock.Setup(expression: x => x.GetResource(resourceId: entities[0].Id))
            .Returns(value: entities[0]);

        resourceEventProcessingServiceMock.Setup(expression: x => x.RaiseResourceDeleteEventAsync(entity: entities[0], userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        resourceProcessingServiceMock.Setup(expression: x => x.DeleteAsync(resourceId: entities[0].Id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAllResourceAsync(deletedResource: entities);

        // Then
        resourceProcessingServiceMock.Verify(expression: x => x.GetResource(resourceId: entities[0].Id), times: Times.Once);
        resourceEventProcessingServiceMock.Verify(expression: x => x.RaiseResourceDeleteEventAsync(entity: entities[0], userId: CurrentUserId), times: Times.Once);
        resourceProcessingServiceMock.Verify(expression: x => x.DeleteAsync(resourceId: entities[0].Id), times: Times.Once);
    }

}