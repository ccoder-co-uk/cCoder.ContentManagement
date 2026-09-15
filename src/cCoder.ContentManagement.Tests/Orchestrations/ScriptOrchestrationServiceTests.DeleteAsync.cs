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

public partial class ScriptOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldGetThenDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        // Given
        int id = 1;
        Script entity = CreateRandomScript();

        scriptProcessingServiceMock.Setup(expression: x => x.GetScript(scriptId: id))
            .Returns(value: entity);

        scriptProcessingServiceMock.Setup(expression: x => x.DeleteAsync(scriptId: id))
            .Returns(value: ValueTask.CompletedTask);

        scriptEventProcessingServiceMock
            .Setup(expression: x => x.RaiseScriptDeleteEventAsync(entity: entity, userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(scriptId: id);

        // Then
        scriptProcessingServiceMock.Verify(expression: x => x.GetScript(scriptId: id), times: Times.Once);
        scriptProcessingServiceMock.Verify(expression: x => x.DeleteAsync(scriptId: id), times: Times.Once);
        scriptEventProcessingServiceMock.Verify(expression: x => x.RaiseScriptDeleteEventAsync(entity: entity, userId: CurrentUserId), times: Times.Once);

        authorizationProcessingServiceMock.Verify(
            expression: service => service.AuthorizeAuthorizationContext(
context:                 It.Is<AuthorizationContext>(match: context =>
                    context.Request.AppId == entity.AppId
                    && context.Request.Privilege == "Script_delete")),
            times: Times.Once);
    }

    [Fact]
    public async Task Script_WhenUserLacksDeletePrivilege_IsNotDeleted()
    {
        // Given
        int id = 1;
        Script entity = CreateRandomScript();

        scriptProcessingServiceMock
            .Setup(expression: service => service.GetScript(scriptId: id))
            .Returns(value: entity);

        authorizationProcessingServiceMock
            .Setup(expression: service => service.AuthorizeAuthorizationContext(
context:                 It.Is<AuthorizationContext>(match: context =>
                    context.Request.AppId == entity.AppId
                    && context.Request.Privilege == "Script_delete")))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () =>
            await orchestrationService.DeleteAsync(scriptId: id);

        // Then
        await action.Should()
            .ThrowAsync<ContentManagementSecurityException>();

        scriptProcessingServiceMock.Verify(
            expression: service => service.GetScript(scriptId: id),
            times: Times.Once);

        scriptProcessingServiceMock.VerifyNoOtherCalls();
        scriptEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}