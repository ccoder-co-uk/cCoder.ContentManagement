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
    public async Task ShouldCallProcessingThenRaiseUpdateEventAsyncWhenUpdateAsync()
    {
        // Given
        Layout entity = CreateRandomLayout();

        layoutProcessingServiceMock.Setup(expression: x => x.UpdateLayoutAsync(updatedLayout: entity))
            .ReturnsAsync(value: entity);

        layoutEventProcessingServiceMock
            .Setup(expression: x => x.RaiseLayoutUpdateEventAsync(entity: entity, userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        Layout result = await orchestrationService.UpdateLayoutAsync(updatedLayout: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        layoutProcessingServiceMock.Verify(expression: x => x.UpdateLayoutAsync(updatedLayout: entity), times: Times.Once);
        layoutEventProcessingServiceMock.Verify(expression: x => x.RaiseLayoutUpdateEventAsync(entity: entity, userId: CurrentUserId), times: Times.Once);

        authorizationProcessingServiceMock.Verify(
            expression: service => service.AuthorizeAuthorizationContext(
                It.Is<AuthorizationContext>(context =>
                    context.Request.AppId == entity.AppId
                    && context.Request.Privilege == "Layout_update")),
            times: Times.Once);

        entity.LastUpdatedBy.Should().Be(CurrentUserId);
    }

    [Fact]
    public async Task Layout_WhenUserLacksUpdatePrivilege_IsNotPersisted()
    {
        // Given
        Layout entity = CreateRandomLayout();

        authorizationProcessingServiceMock
            .Setup(expression: service => service.AuthorizeAuthorizationContext(
                It.Is<AuthorizationContext>(context =>
                    context.Request.AppId == entity.AppId
                    && context.Request.Privilege == "Layout_update")))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () =>
            await orchestrationService.UpdateLayoutAsync(updatedLayout: entity);

        // Then
        await action.Should()
            .ThrowAsync<ContentManagementSecurityException>();

        layoutProcessingServiceMock.VerifyNoOtherCalls();
        layoutEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}