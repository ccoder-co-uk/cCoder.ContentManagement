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

public partial class TemplateOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldGetThenDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        // Given
        int id = 1;
        Template entity = CreateRandomTemplate();

        templateProcessingServiceMock.Setup(expression: x => x.GetTemplate(templateId: id))
            .Returns(value: entity);

        templateProcessingServiceMock.Setup(expression: x => x.DeleteAsync(templateId: id))
            .Returns(value: ValueTask.CompletedTask);

        templateEventProcessingServiceMock
            .Setup(expression: x => x.RaiseTemplateDeleteEventAsync(entity: entity, userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(templateId: id);

        // Then
        templateProcessingServiceMock.Verify(expression: x => x.GetTemplate(templateId: id), times: Times.Once);
        templateProcessingServiceMock.Verify(expression: x => x.DeleteAsync(templateId: id), times: Times.Once);
        templateEventProcessingServiceMock.Verify(expression: x => x.RaiseTemplateDeleteEventAsync(entity: entity, userId: CurrentUserId), times: Times.Once);

        authorizationProcessingServiceMock.Verify(
            expression: service => service.AuthorizeAuthorizationContext(
                context: It.Is<AuthorizationContext>(match: context =>
                    context.Request.AppId == entity.AppId
                    && context.Request.Privilege == "Template_delete")),
            times: Times.Once);
    }

    [Fact]
    public async Task Template_WhenUserLacksDeletePrivilege_IsNotDeleted()
    {
        // Given
        int id = 1;
        Template entity = CreateRandomTemplate();

        templateProcessingServiceMock
            .Setup(expression: service => service.GetTemplate(templateId: id))
            .Returns(value: entity);

        authorizationProcessingServiceMock
            .Setup(expression: service => service.AuthorizeAuthorizationContext(
                context: It.Is<AuthorizationContext>(match: context =>
                    context.Request.AppId == entity.AppId
                    && context.Request.Privilege == "Template_delete")))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () =>
            await orchestrationService.DeleteAsync(templateId: id);

        // Then
        await action
            .Should()
            .ThrowAsync<ContentManagementSecurityException>();

        templateProcessingServiceMock.Verify(
            expression: service => service.GetTemplate(templateId: id),
            times: Times.Once);

        templateProcessingServiceMock.VerifyNoOtherCalls();
        templateEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}