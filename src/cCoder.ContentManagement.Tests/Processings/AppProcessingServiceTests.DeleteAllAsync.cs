// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class AppProcessingServiceTests
{
    [Fact]
    public async Task ShouldDeleteEachAppWhenUserIsAppAdminForDeleteAllAsync()
    {
        // Given
        User admin = TestUsers.WithPrivilege(privilege: "app_admin", appId: 1);
        App app = CreateRandomApp();
        app.Id = 1;
        currentUser = admin;

        appServiceMock.Setup(expression: x => x.DeleteAsync(appId: app.Id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await appProcessingService.DeleteAllAppAsync(deletedApp: new[] { app });

        // Then
        appServiceMock.Verify(expression: x => x.DeleteAsync(appId: app.Id), times: Times.Once);
        appServiceMock.VerifyNoOtherCalls();
        authorizationManagerMock.VerifyNoOtherCalls();
    }

}