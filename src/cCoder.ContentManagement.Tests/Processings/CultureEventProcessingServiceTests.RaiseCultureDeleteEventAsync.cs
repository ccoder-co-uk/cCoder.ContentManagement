// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class CultureEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseCultureDeleteEventAsync()
    {
        // Given
        Culture entity = CreateRandomCulture();

        cultureEventServiceMock
            .Setup(expression: x => x.RaiseCultureDeleteEventAsync(
                entity: entity,
                userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseCultureDeleteEventAsync(
            culture: entity,
            userId: CurrentUserId);

        // Then
        cultureEventServiceMock.Verify(expression: x => x.RaiseCultureDeleteEventAsync(
            entity: entity,
            userId: CurrentUserId), times: Times.Once);

        cultureEventServiceMock.VerifyNoOtherCalls();
    }

}