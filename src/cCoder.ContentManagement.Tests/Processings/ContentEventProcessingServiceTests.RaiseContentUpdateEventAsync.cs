// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class ContentEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseContentUpdateEventAsync()
    {
        // Given
        Content entity = CreateRandomContent();

        contentEventServiceMock
            .Setup(expression: x => x.RaiseContentUpdateEventAsync(
                entity: entity,
                userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseContentUpdateEventAsync(
            content: entity,
            userId: CurrentUserId);

        // Then
        contentEventServiceMock.Verify(expression: x => x.RaiseContentUpdateEventAsync(
            entity: entity,
            userId: CurrentUserId), times: Times.Once);

        contentEventServiceMock.VerifyNoOtherCalls();
    }

}