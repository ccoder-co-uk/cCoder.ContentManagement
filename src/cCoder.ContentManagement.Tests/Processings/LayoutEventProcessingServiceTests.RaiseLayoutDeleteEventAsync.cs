// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class LayoutEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseLayoutDeleteEventAsync()
    {
        // Given
        Layout entity = CreateRandomLayout();

        layoutEventServiceMock
            .Setup(expression: x => x.RaiseLayoutDeleteEventAsync(
                entity: entity,
                userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseLayoutDeleteEventAsync(
            layout: entity,
            userId: CurrentUserId);

        // Then
        layoutEventServiceMock.Verify(expression: x => x.RaiseLayoutDeleteEventAsync(
            entity: entity,
            userId: CurrentUserId), times: Times.Once);

        layoutEventServiceMock.VerifyNoOtherCalls();
    }

}