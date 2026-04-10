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
    public async Task ShouldUpdatePageOrderAndSaveWhenAppExists()
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

        App incomingApp = CreateRandomApp();
        incomingApp.Id = 1;
        incomingApp.Pages =
        [
            new Page
            {
                Id = 5,
                Order = 7,
                ParentId = 3,
                Name = "Home",
                AppId = incomingApp.Id,
                PageInfo = [],
                Roles = [],
                Contents = [],
                Pages = [],
            },
        ];
        appServiceMock
            .Setup(x => x.UpdatePageOrderAsync(incomingApp.Id, incomingApp.Pages))
            .Returns(ValueTask.CompletedTask);

        // When
        await appProcessingService.UpdatePageOrderAsync(incomingApp.Id, incomingApp);

        // Then
        appServiceMock.Verify(x => x.UpdatePageOrderAsync(incomingApp.Id, incomingApp.Pages), Times.Once);
        appServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowTaskCanceledExceptionWhenAppDoesNotExistForUpdatePageOrder()
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

        App incomingApp = CreateRandomApp();
        incomingApp.Id = 1;
        incomingApp.Pages = [];
        appServiceMock
            .Setup(x => x.UpdatePageOrderAsync(incomingApp.Id, incomingApp.Pages))
            .Returns(ValueTask.FromException(new TaskCanceledException("App not found")));

        // When
        Func<Task> act = async () =>
            await appProcessingService.UpdatePageOrderAsync(incomingApp.Id, incomingApp);

        // Then
        await act.Should().ThrowAsync<TaskCanceledException>().WithMessage("App not found");
        appServiceMock.Verify(x => x.UpdatePageOrderAsync(incomingApp.Id, incomingApp.Pages), Times.Once);
        appServiceMock.VerifyNoOtherCalls();
    }

}
















