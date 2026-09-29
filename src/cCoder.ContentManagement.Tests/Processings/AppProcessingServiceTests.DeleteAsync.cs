// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class AppProcessingServiceTests
{
    [Fact]
    public async Task ShouldDeleteAppWhenUserIsAppAdminForDeleteAsync()
    {
        // Given
        User actor = TestUsers.WithPrivilege(privilege: "app_admin", appId: 5);

        currentUser = actor;

        appServiceMock.Setup(expression: x => x.DeleteAsync(appId: 5))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await appProcessingService.DeleteAsync(appId: 5);

        // Then
        appServiceMock.Verify(expression: x => x.DeleteAsync(appId: 5), times: Times.Once);
        appServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldDelegateToServiceWhenUserIsNotAppAdminForDeleteAsync()
    {
        // Given
        currentUser = TestUsers.WithoutPrivileges();

        appServiceMock.Setup(expression: x => x.DeleteAsync(appId: 5))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await appProcessingService.DeleteAsync(appId: 5);

        // Then
        appServiceMock.Verify(expression: x => x.DeleteAsync(appId: 5), times: Times.Once);
        appServiceMock.VerifyNoOtherCalls();
        authorizationManagerMock.VerifyNoOtherCalls();
    }

}