// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using System.Security;
using cCoder.ContentManagement.Brokers.Loggings;
using cCoder.ContentManagement.Brokers.OData;
using cCoder.ContentManagement.Exposures.Controllers;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Models.Exceptions;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class AppOrchestrationServiceTests
{
    [Fact]
    public async Task PutApp_WhenAuthorizationWrapsAccessDenied_ReturnsForbidden()
    {
        // Given
        App app = CreateRandomApp();

        AppController controller = new(
            service: orchestrationService,
            loggingBroker: Mock.Of<ILoggingBroker>(),
            oDataResultBroker: Mock.Of<IODataResultBroker>());

        authorizationProcessingServiceMock
            .Setup(expression: service => service.AuthorizeAuthorizationContext(
                context: It.Is<AuthorizationContext>(match: context =>
                    context.Request.AppId == app.Id && context.Request.Privilege == "app_update")))
            .Throws(exception: new ContentManagementServiceException(
                innerException: new SecurityException(message: "Access Denied!")));

        // When
        IActionResult result = await controller.Put(key: app.Id, updatedApp: app);

        // Then
        ObjectResult response = Assert.IsType<ObjectResult>(@object: result);

        Assert.Equal(expected: StatusCodes.Status403Forbidden, actual: response.StatusCode);
        appProcessingServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateApp_WhenAuthorizationWrapsAccessDenied_PreservesSecurityOutcome()
    {
        // Given
        App app = CreateRandomApp();

        authorizationProcessingServiceMock
            .Setup(expression: service => service.AuthorizeAuthorizationContext(
                context: It.Is<AuthorizationContext>(match: context =>
                    context.Request.AppId == app.Id && context.Request.Privilege == "app_update")))
            .Throws(exception: new ContentManagementServiceException(
                innerException: new SecurityException(message: "Access Denied!")));

        // When
        Func<Task> update = async () => await orchestrationService.UpdateAppAsync(updatedApp: app);

        // Then
        await update.Should()
            .ThrowAsync<ContentManagementSecurityException>();

        appProcessingServiceMock.VerifyNoOtherCalls();

        appEventProcessingServiceMock.Verify(
            expression: service => service.RaiseAppUpdateEventAsync(
                app: It.IsAny<App>(), userId: It.IsAny<string>()),
            times: Times.Never);
    }

    [Fact]
    public async Task AddApp_WhenAuthorizationIsDenied_DoesNotPersistOrRaiseEvent()
    {
        // Given
        App app = CreateRandomApp();

        authorizationProcessingServiceMock
            .Setup(expression: service => service.AuthorizeAuthorizationContext(
                context: It.Is<AuthorizationContext>(match: context =>
                    context.Request.AppId == null
                    && context.Request.Privilege == "app_create")))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () =>
            await orchestrationService.AddAppAsync(newApp: app);

        // Then
        await action.Should()
            .ThrowAsync<SecurityException>();

        appProcessingServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateApp_WhenAuthorizationIsDenied_DoesNotPersistOrRaiseEvent()
    {
        // Given
        App app = CreateRandomApp();

        authorizationProcessingServiceMock
            .Setup(expression: service => service.AuthorizeAuthorizationContext(
                context: It.Is<AuthorizationContext>(match: context =>
                    context.Request.AppId == app.Id
                    && context.Request.Privilege == "app_update")))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () =>
            await orchestrationService.UpdateAppAsync(updatedApp: app);

        // Then
        await action.Should()
            .ThrowAsync<SecurityException>();

        appProcessingServiceMock.VerifyNoOtherCalls();
    }
}