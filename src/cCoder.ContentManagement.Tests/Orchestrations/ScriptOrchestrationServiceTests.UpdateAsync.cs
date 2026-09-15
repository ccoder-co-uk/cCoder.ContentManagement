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
    public async Task ShouldCallProcessingThenRaiseUpdateEventAsyncWhenUpdateAsync()
    {
        // Given
        Script entity = CreateRandomScript();

        scriptProcessingServiceMock.Setup(expression: x => x.UpdateScriptAsync(updatedScript: entity))
            .ReturnsAsync(value: entity);

        scriptEventProcessingServiceMock
            .Setup(expression: x => x.RaiseScriptUpdateEventAsync(entity: entity, userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        Script result = await orchestrationService.UpdateScriptAsync(updatedScript: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        scriptProcessingServiceMock.Verify(expression: x => x.UpdateScriptAsync(updatedScript: entity), times: Times.Once);
        scriptEventProcessingServiceMock.Verify(expression: x => x.RaiseScriptUpdateEventAsync(entity: entity, userId: CurrentUserId), times: Times.Once);

        authorizationProcessingServiceMock.Verify(
            expression: service => service.AuthorizeAuthorizationContext(
context:                 It.Is<AuthorizationContext>(match: context =>
                    context.Request.AppId == entity.AppId
                    && context.Request.Privilege == "Script_update")),
            times: Times.Once);

        entity.LastUpdatedBy.Should()
            .Be(expected: CurrentUserId);
    }

    [Fact]
    public async Task Script_WhenUserLacksUpdatePrivilege_IsNotPersisted()
    {
        // Given
        Script entity = CreateRandomScript();

        authorizationProcessingServiceMock
            .Setup(expression: service => service.AuthorizeAuthorizationContext(
context:                 It.Is<AuthorizationContext>(match: context =>
                    context.Request.AppId == entity.AppId
                    && context.Request.Privilege == "Script_update")))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () =>
            await orchestrationService.UpdateScriptAsync(updatedScript: entity);

        // Then
        await action.Should()
            .ThrowAsync<ContentManagementSecurityException>();

        scriptProcessingServiceMock.VerifyNoOtherCalls();
        scriptEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}