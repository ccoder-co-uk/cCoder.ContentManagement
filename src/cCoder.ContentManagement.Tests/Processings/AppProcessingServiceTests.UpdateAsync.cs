using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Security;
using ComponentRenderParams = cCoder.ContentManagement.Models.ComponentRenderParams;
using Config = cCoder.ContentManagement.Models.Config;
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
            .Setup(x => x.Authorize(It.IsAny<int?>(), It.IsAny<string>()))
            .Callback((int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId, privilege) ?? false))
                    throw new SecurityException("Access Denied!");
            });

        authorizationBrokerMock
            .Setup(x => x.IsAdminOfApp(It.IsAny<int>()))
            .Returns((int appId) => currentUser?.IsAdminOfApp(appId) ?? false);

        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(() => currentUser);

        User admin = TestUsers.WithPrivilege("app_admin", 1);
        App dbApp = CreateRandomApp();
        dbApp.Id = 1;
        dbApp.Cultures = null!;
        App app = CreateRandomApp();
        app.Id = dbApp.Id;
        app.Cultures = null!;

        currentUser = admin;
        appServiceMock.Setup(x => x.Get(dbApp.Id, true)).Returns(dbApp);
        appServiceMock.Setup(x => x.UpdateAsync(dbApp)).ReturnsAsync(dbApp);
        appEventProcessingServiceMock
            .Setup(x => x.RaiseAppUpdateEventAsync(dbApp))
            .Returns(ValueTask.CompletedTask);

        // When
        App result = await appProcessingService.UpdateAsync(app);

        // Then
        result.Should().BeSameAs(dbApp);
        appServiceMock.Verify(x => x.Get(dbApp.Id, true), Times.Once);
        appServiceMock.Verify(x => x.UpdateAsync(dbApp), Times.Once);
        appServiceMock.VerifyNoOtherCalls();
        appEventProcessingServiceMock.Verify(x => x.RaiseAppUpdateEventAsync(dbApp), Times.Once);
        appEventProcessingServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserIsNotAppAdminForUpdateAsync()
    {
        // Given
        authorizationBrokerMock
            .Setup(x => x.Authorize(It.IsAny<int?>(), It.IsAny<string>()))
            .Callback((int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId, privilege) ?? false))
                    throw new SecurityException("Access Denied!");
            });

        authorizationBrokerMock
            .Setup(x => x.IsAdminOfApp(It.IsAny<int>()))
            .Returns((int appId) => currentUser?.IsAdminOfApp(appId) ?? false);

        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(() => currentUser);

        User actor = TestUsers.WithoutPrivileges();
        App app = CreateRandomApp();
        app.Id = 1;

        currentUser = actor;
        appServiceMock.Setup(x => x.Get(app.Id, true)).Returns(app);
        appServiceMock
            .Setup(x => x.UpdateAsync(It.IsAny<App>()))
            .ThrowsAsync(new SecurityException("Access Denied!"));

        // When
        Func<Task> act = async () => await appProcessingService.UpdateAsync(app);

        // Then
        await act.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        appServiceMock.Verify(x => x.Get(app.Id, true), Times.Once);
        appServiceMock.Verify(x => x.UpdateAsync(It.IsAny<App>()), Times.Once);
        appServiceMock.VerifyNoOtherCalls();
        appEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}
















