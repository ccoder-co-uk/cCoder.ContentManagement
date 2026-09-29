// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class ResourceEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseResourceUpdateEventAsync()
    {
        // Given
        Resource entity = CreateRandomResource();

        resourceEventServiceMock
            .Setup(expression: x => x.RaiseResourceUpdateEventAsync(
                entity: entity,
                userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseResourceUpdateEventAsync(
            resource: entity,
            userId: CurrentUserId);

        // Then
        resourceEventServiceMock.Verify(expression: x => x.RaiseResourceUpdateEventAsync(
            entity: entity,
            userId: CurrentUserId), times: Times.Once);

        resourceEventServiceMock.VerifyNoOtherCalls();
    }

}