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

public partial class LayoutOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldGetThenDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        // Given
        int id = 1;
        Layout entity = CreateRandomLayout();

        layoutProcessingServiceMock.Setup(expression: x => x.GetLayout(layoutId: id))
            .Returns(value: entity);

        layoutProcessingServiceMock.Setup(expression: x => x.DeleteAsync(layoutId: id))
            .Returns(value: ValueTask.CompletedTask);

        layoutEventProcessingServiceMock
            .Setup(expression: x => x.RaiseLayoutDeleteEventAsync(entity: entity, userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(layoutId: id);

        // Then
        layoutProcessingServiceMock.Verify(expression: x => x.GetLayout(layoutId: id), times: Times.Once);
        layoutProcessingServiceMock.Verify(expression: x => x.DeleteAsync(layoutId: id), times: Times.Once);
        layoutEventProcessingServiceMock.Verify(expression: x => x.RaiseLayoutDeleteEventAsync(entity: entity, userId: CurrentUserId), times: Times.Once);

        authorizationProcessingServiceMock.Verify(
            expression: service => service.AuthorizeAuthorizationContext(
context:                 It.Is<AuthorizationContext>(match: context =>
                    context.Request.AppId == entity.AppId
                    && context.Request.Privilege == "Layout_delete")),
            times: Times.Once);
    }

    [Fact]
    public async Task Layout_WhenUserLacksDeletePrivilege_IsNotDeleted()
    {
        // Given
        int id = 1;
        Layout entity = CreateRandomLayout();

        layoutProcessingServiceMock
            .Setup(expression: service => service.GetLayout(layoutId: id))
            .Returns(value: entity);

        authorizationProcessingServiceMock
            .Setup(expression: service => service.AuthorizeAuthorizationContext(
context:                 It.Is<AuthorizationContext>(match: context =>
                    context.Request.AppId == entity.AppId
                    && context.Request.Privilege == "Layout_delete")))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () =>
            await orchestrationService.DeleteAsync(layoutId: id);

        // Then
        await action.Should()
            .ThrowAsync<ContentManagementSecurityException>();

        layoutProcessingServiceMock.Verify(
            expression: service => service.GetLayout(layoutId: id),
            times: Times.Once);

        layoutProcessingServiceMock.VerifyNoOtherCalls();
        layoutEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}