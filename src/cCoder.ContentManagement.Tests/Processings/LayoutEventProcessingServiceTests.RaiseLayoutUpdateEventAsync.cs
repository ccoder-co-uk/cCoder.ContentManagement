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
    public async Task ShouldPassThroughCallWhenRaiseLayoutUpdateEventAsync()
    {
        // Given
        Layout entity = CreateRandomLayout();

        layoutEventServiceMock
            .Setup(expression: x => x.RaiseLayoutUpdateEventAsync(
                entity: entity,
                userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseLayoutUpdateEventAsync(
            layout: entity,
            userId: CurrentUserId);

        // Then
        layoutEventServiceMock.Verify(expression: x => x.RaiseLayoutUpdateEventAsync(
            entity: entity,
            userId: CurrentUserId), times: Times.Once);

        layoutEventServiceMock.VerifyNoOtherCalls();
    }

}