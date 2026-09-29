// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class ComponentEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseComponentUpdateEventAsync()
    {
        // Given
        Component entity = CreateRandomComponent();

        componentEventServiceMock
            .Setup(expression: x => x.RaiseComponentUpdateEventAsync(
                entity: entity,
                userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseComponentUpdateEventAsync(
            component: entity,
            userId: CurrentUserId);

        // Then
        componentEventServiceMock.Verify(expression: x => x.RaiseComponentUpdateEventAsync(
            entity: entity,
            userId: CurrentUserId), times: Times.Once);

        componentEventServiceMock.VerifyNoOtherCalls();
    }

}