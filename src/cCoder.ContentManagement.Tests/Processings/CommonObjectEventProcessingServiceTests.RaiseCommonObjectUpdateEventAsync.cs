// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models;
using Moq;
using Xunit;



namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class CommonObjectEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseCommonObjectUpdateEventAsync()
    {
        // Given
        CommonObject entity = CreateRandomCommonObject();

        commonObjectEventServiceMock
            .Setup(expression: x => x.RaiseCommonObjectUpdateEventAsync(
                entity: entity,
                userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseCommonObjectUpdateEventAsync(
            commonObject: entity,
            userId: CurrentUserId);

        // Then
        commonObjectEventServiceMock.Verify(expression: x => x.RaiseCommonObjectUpdateEventAsync(
            entity: entity,
            userId: CurrentUserId), times: Times.Once);

        commonObjectEventServiceMock.VerifyNoOtherCalls();
    }

}