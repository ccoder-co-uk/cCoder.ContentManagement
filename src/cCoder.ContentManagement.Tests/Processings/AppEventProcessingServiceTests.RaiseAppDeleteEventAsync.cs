// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class AppEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseAppDeleteEventAsync()
    {
        // Given
        App app = CreateRandomApp();

        appEventServiceMock
            .Setup(expression: x => x.RaiseAppDeleteEventAsync(
                app: app,
                userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseAppDeleteEventAsync(
            app: app,
            userId: CurrentUserId);

        // Then
        appEventServiceMock.Verify(expression: x => x.RaiseAppDeleteEventAsync(
            app: app,
            userId: CurrentUserId), times: Times.Once);

        appEventServiceMock.VerifyNoOtherCalls();
    }

}