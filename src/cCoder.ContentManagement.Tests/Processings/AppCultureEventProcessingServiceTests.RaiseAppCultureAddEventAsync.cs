// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class AppCultureEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseAppCultureAddEventAsync()
    {
        // Given
        AppCulture entity = CreateRandomAppCulture();

        appCultureEventServiceMock
            .Setup(expression: x => x.RaiseAppCultureAddEventAsync(
                entity: entity,
                userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseAppCultureAddEventAsync(
            appCulture: entity,
            userId: CurrentUserId);

        // Then
        appCultureEventServiceMock.Verify(expression: x => x.RaiseAppCultureAddEventAsync(
            entity: entity,
            userId: CurrentUserId), times: Times.Once);

        appCultureEventServiceMock.VerifyNoOtherCalls();
    }

}