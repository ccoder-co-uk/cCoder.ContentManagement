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
    public async Task ShouldPassThroughCallWhenRaiseComponentDeleteEventAsync()
    {
        // Given
        Component entity = CreateRandomComponent();

        componentEventServiceMock
            .Setup(expression: x => x.RaiseComponentDeleteEventAsync(
                entity: entity,
                userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseComponentDeleteEventAsync(
            component: entity,
            userId: CurrentUserId);

        // Then
        componentEventServiceMock.Verify(expression: x => x.RaiseComponentDeleteEventAsync(
            entity: entity,
            userId: CurrentUserId), times: Times.Once);

        componentEventServiceMock.VerifyNoOtherCalls();
    }

}