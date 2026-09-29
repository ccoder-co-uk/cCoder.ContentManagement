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
    public async Task ShouldPassThroughCallWhenRaiseCultureAddEventAsync()
    {
        // Given
        Culture entity = CreateRandomCulture();

        cultureEventServiceMock
            .Setup(expression: x => x.RaiseCultureAddEventAsync(
                entity: entity,
                userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseCultureAddEventAsync(
            culture: entity,
            userId: CurrentUserId);

        // Then
        cultureEventServiceMock.Verify(expression: x => x.RaiseCultureAddEventAsync(
            entity: entity,
            userId: CurrentUserId), times: Times.Once);

        cultureEventServiceMock.VerifyNoOtherCalls();
    }

}