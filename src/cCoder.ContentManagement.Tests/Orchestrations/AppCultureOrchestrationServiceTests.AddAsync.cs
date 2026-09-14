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
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Models.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class AppCultureOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        AppCulture entity = CreateRandomAppCulture();

        appCultureProcessingServiceMock.Setup(expression: x => x.AddAppCultureAsync(newAppCulture: entity))
            .ReturnsAsync(value: entity);

        appCultureEventProcessingServiceMock
            .Setup(expression: x => x.RaiseAppCultureAddEventAsync(entity: entity, userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        AppCulture result = await orchestrationService.AddAppCultureAsync(newAppCulture: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        appCultureProcessingServiceMock.Verify(expression: x => x.AddAppCultureAsync(newAppCulture: entity), times: Times.Once);
        appCultureEventProcessingServiceMock.Verify(expression: x => x.RaiseAppCultureAddEventAsync(entity: entity, userId: CurrentUserId), times: Times.Once);

        authorizationProcessingServiceMock.Verify(
            expression: service => service.AuthorizeAuthorizationContext(
                It.Is<AuthorizationContext>(context =>
                    context.Request.AppId == entity.AppId
                    && context.Request.Privilege == "AppCulture_create")),
            times: Times.Once);
    }

    [Fact]
    public async Task AppCulture_WhenUserLacksCreatePrivilege_IsNotPersisted()
    {
        // Given
        AppCulture entity = CreateRandomAppCulture();

        authorizationProcessingServiceMock
            .Setup(expression: service => service.AuthorizeAuthorizationContext(
                It.Is<AuthorizationContext>(context =>
                    context.Request.AppId == entity.AppId
                    && context.Request.Privilege == "AppCulture_create")))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () =>
            await orchestrationService.AddAppCultureAsync(newAppCulture: entity);

        // Then
        await action.Should()
            .ThrowAsync<ContentManagementSecurityException>();

        appCultureProcessingServiceMock.VerifyNoOtherCalls();
        appCultureEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}