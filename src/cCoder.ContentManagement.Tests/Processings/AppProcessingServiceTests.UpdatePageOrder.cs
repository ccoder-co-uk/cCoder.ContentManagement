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
    public async Task ShouldUpdatePageOrderAndSaveWhenAppExists()
    {
        // Given
        currentUser = TestUsers.WithPrivilege(
            privilege: "app_update",
            appId: 1);

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

        Page existingPage = new()
        {
            Id = incomingApp.Pages.Single().Id,
            AppId = incomingApp.Id
        };

        pageBrokerMock
            .Setup(expression: broker => broker.GetAllPages(ignoreFilters: true))
            .Returns(value: new[] { existingPage }.AsQueryable());

        pageBrokerMock
            .Setup(expression: broker => broker.UpdatePageAsync(updatedPage: existingPage))
            .ReturnsAsync(value: existingPage);

        // When
        await appProcessingService.UpdatePageOrderAppAsync(key: incomingApp.Id, updatedApp: incomingApp);

        // Then
        pageBrokerMock.Verify(
            expression: broker => broker.GetAllPages(ignoreFilters: true),
            times: Times.Once);

        pageBrokerMock.Verify(
            expression: broker => broker.UpdatePageAsync(updatedPage: existingPage),
            times: Times.Once);

        appServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowTaskCanceledExceptionWhenAppDoesNotExistForUpdatePageOrder()
    {
        // Given
        currentUser = TestUsers.WithPrivilege(
            privilege: "app_update",
            appId: 1);

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

        App incomingApp = CreateRandomApp();
        incomingApp.Id = 1;
        incomingApp.Pages = [];

        pageBrokerMock
            .Setup(expression: broker => broker.GetAllPages(ignoreFilters: true))
            .Throws(exception: new TaskCanceledException(message: "App not found"));

        // When

        Func<Task> act = async () =>
            await appProcessingService.UpdatePageOrderAppAsync(key: incomingApp.Id, updatedApp: incomingApp);

        // Then

        await act.Should()
            .ThrowAsync<TaskCanceledException>()
            .WithMessage(expectedWildcardPattern: "App not found");

        pageBrokerMock.Verify(
            expression: broker => broker.GetAllPages(ignoreFilters: true),
            times: Times.Once);

        appServiceMock.VerifyNoOtherCalls();
    }

}