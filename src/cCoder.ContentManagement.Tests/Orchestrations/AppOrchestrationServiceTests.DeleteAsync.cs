// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;
using cCoder.ContentManagement.Models;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class AppOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldGetAuthorizeAndRaiseEventWhenDeleteAppAsync()
    {
        // Given
        int id = 1;
        App app = CreateRandomApp();
        app.Id = id;
        app.Roles = [new Role { Id = Guid.NewGuid(), AppId = id, Users = [] }];

        authorizationProcessingServiceMock
            .Setup(expression: x => x.AuthorizeAuthorizationContext(
                context: It.Is<AuthorizationContext>(match: context =>
                    context.Request.AppId == id
                    && context.Request.Privilege == "app_delete")));

        appProcessingServiceMock.Setup(expression: x => x.GetAppForDelete(appId: id))
            .Returns(value: app);

        appEventProcessingServiceMock
            .Setup(expression: service => service.RaiseAppDeleteEventAsync(
                app: app,
                userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAppAsync(appId: id);

        // Then
        authorizationProcessingServiceMock.Verify(
            expression: x => x.AuthorizeAuthorizationContext(
                context: It.Is<AuthorizationContext>(match: context =>
                    context.Request.AppId == id
                    && context.Request.Privilege == "app_delete")),
            times: Times.Once);

        appProcessingServiceMock.Verify(expression: x => x.GetAppForDelete(appId: id), times: Times.Once);

        appEventProcessingServiceMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldDeleteAppWhenHandlingAppDeleteAsync()
    {
        // Given
        App app = CreateRandomApp();

        appProcessingServiceMock
            .Setup(expression: service => service.DeleteAsync(appId: app.Id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.HandleAppDeleteAsync(app: app);

        // Then
        appProcessingServiceMock.Verify(
            expression: service => service.DeleteAsync(appId: app.Id),
            times: Times.Once);
    }

}