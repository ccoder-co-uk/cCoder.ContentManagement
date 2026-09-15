// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.ContentManagement.Models;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class AppOrchestrationServiceTests
{
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