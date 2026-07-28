// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Security;
using ComponentRenderParams = cCoder.ContentManagement.Models.ComponentRenderParams;
using Config = cCoder.ContentManagement.Models.ContentManagementConfiguration;
using PageRenderParams = cCoder.ContentManagement.Models.PageRenderParams;
using PageRoleInfo = cCoder.ContentManagement.Models.PageRoleInfo;
using RenderParams = cCoder.ContentManagement.Models.RenderParams;
using RenderResult = cCoder.ContentManagement.Models.RenderResult;
using TemplateRenderParams = cCoder.ContentManagement.Models.TemplateRenderParams;
using System.Security;



using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class AppProcessingServiceTests
{
    [Fact]
    public async Task ShouldUpdateAppWhenUserIsAppAdminForUpdateAsync()
    {
        // Given
        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: It.IsAny<int?>(), privilege: It.IsAny<string>()))
            .Callback(action: (int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId: appId, operation: privilege) ?? false))
                {
                    throw new SecurityException(message: "Access Denied!");
                }
            });

        authorizationBrokerMock
            .Setup(expression: x => x.IsAdminOfApp(appId: It.IsAny<int>()))
            .Returns(valueFunction: (int appId) => currentUser?.IsAdminOfApp(appId: appId) ?? false);

        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(valueFunction: () => currentUser);

        User admin = TestUsers.WithPrivilege(privilege: "app_admin", appId: 1);
        App dbApp = CreateRandomApp();
        dbApp.Id = 1;
        dbApp.Cultures = null!;
        App app = CreateRandomApp();
        app.Id = dbApp.Id;
        app.Cultures = null!;

        currentUser = admin;

        appServiceMock.Setup(expression: x => x.GetApp(appId: dbApp.Id, ignoreFilters: true))
            .Returns(value: dbApp);

        appServiceMock.Setup(expression: x => x.UpdateAppAsync(updatedApp: dbApp))
            .ReturnsAsync(value: dbApp);

        // When
        App result = await appProcessingService.UpdateAppAsync(updatedApp: app);

        // Then

        result.Should()
            .BeSameAs(expected: dbApp);

        appServiceMock.Verify(expression: x => x.GetApp(appId: dbApp.Id, ignoreFilters: true), times: Times.Once);
        appServiceMock.Verify(expression: x => x.UpdateAppAsync(updatedApp: dbApp), times: Times.Once);
        appServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserIsNotAppAdminForUpdateAsync()
    {
        // Given
        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: It.IsAny<int?>(), privilege: It.IsAny<string>()))
            .Callback(action: (int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId: appId, operation: privilege) ?? false))
                {
                    throw new SecurityException(message: "Access Denied!");
                }
            });

        authorizationBrokerMock
            .Setup(expression: x => x.IsAdminOfApp(appId: It.IsAny<int>()))
            .Returns(valueFunction: (int appId) => currentUser?.IsAdminOfApp(appId: appId) ?? false);

        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(valueFunction: () => currentUser);

        User actor = TestUsers.WithoutPrivileges();
        App app = CreateRandomApp();
        app.Id = 1;

        currentUser = actor;

        appServiceMock.Setup(expression: x => x.GetApp(appId: app.Id, ignoreFilters: true))
            .Returns(value: app);

        appServiceMock
            .Setup(expression: x => x.UpdateAppAsync(updatedApp: It.IsAny<App>()))
            .ThrowsAsync(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> act = async () => await appProcessingService.UpdateAppAsync(updatedApp: app);

        // Then

        await act.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        appServiceMock.Verify(expression: x => x.GetApp(appId: app.Id, ignoreFilters: true), times: Times.Once);
        appServiceMock.Verify(expression: x => x.UpdateAppAsync(updatedApp: It.IsAny<App>()), times: Times.Once);
        appServiceMock.VerifyNoOtherCalls();
    }

}