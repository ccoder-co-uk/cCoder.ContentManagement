// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class AppOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnAuthorizationResultWhenCheckingAppAdministrator()
    {
        // Given
        const int appId = 1;
        const string userName = "user-id";
        const bool expectedResult = true;

        authorizationProcessingServiceMock
            .Setup(expression: broker => broker.IsAdmin(appId: appId, userName: userName))
            .Returns(value: expectedResult);

        // When
        bool actualResult = orchestrationService.IsAdminApp(
            appId: appId,
            userName: userName);

        // Then
        actualResult.Should()
            .Be(expected: expectedResult);

        authorizationProcessingServiceMock.Verify(
            expression: broker => broker.IsAdmin(appId: appId, userName: userName),
            times: Times.Once);

        authorizationProcessingServiceMock.VerifyNoOtherCalls();
        appProcessingServiceMock.VerifyNoOtherCalls();
        appEventProcessingServiceMock.VerifyNoOtherCalls();
    }
}