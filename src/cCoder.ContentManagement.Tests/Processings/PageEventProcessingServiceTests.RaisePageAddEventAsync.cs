// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PageEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaisePageAddEventAsync()
    {
        // Given
        Page entity = CreateRandomPage();

        pageEventServiceMock
            .Setup(expression: x => x.RaisePageAddEventAsync(
                entity: entity,
                userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaisePageAddEventAsync(
            page: entity,
            userId: CurrentUserId);

        // Then
        pageEventServiceMock.Verify(expression: x => x.RaisePageAddEventAsync(
            entity: entity,
            userId: CurrentUserId), times: Times.Once);

        pageEventServiceMock.VerifyNoOtherCalls();
    }

}