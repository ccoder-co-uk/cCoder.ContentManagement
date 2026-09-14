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

public partial class ResourceOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseUpdateEventAsyncWhenUpdateAsync()
    {
        // Given
        Resource entity = CreateRandomResource();

        resourceProcessingServiceMock.Setup(expression: x => x.UpdateResourceAsync(updatedResource: entity))
            .ReturnsAsync(value: entity);

        resourceEventProcessingServiceMock
            .Setup(expression: x => x.RaiseResourceUpdateEventAsync(entity: entity, userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        Resource result = await orchestrationService.UpdateResourceAsync(updatedResource: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        resourceProcessingServiceMock.Verify(expression: x => x.UpdateResourceAsync(updatedResource: entity), times: Times.Once);
        resourceEventProcessingServiceMock.Verify(expression: x => x.RaiseResourceUpdateEventAsync(entity: entity, userId: CurrentUserId), times: Times.Once);

        authorizationProcessingServiceMock.Verify(
            expression: service => service.AuthorizeAuthorizationContext(
                It.Is<AuthorizationContext>(context =>
                    context.Request.AppId == entity.AppId
                    && context.Request.Privilege == "Resource_update")),
            times: Times.Once);

        entity.LastUpdatedBy.Should().Be(CurrentUserId);
    }

    [Fact]
    public async Task Resource_WhenUserLacksUpdatePrivilege_IsNotPersisted()
    {
        // Given
        Resource entity = CreateRandomResource();

        authorizationProcessingServiceMock
            .Setup(expression: service => service.AuthorizeAuthorizationContext(
                It.Is<AuthorizationContext>(context =>
                    context.Request.AppId == entity.AppId
                    && context.Request.Privilege == "Resource_update")))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () =>
            await orchestrationService.UpdateResourceAsync(updatedResource: entity);

        // Then
        await action.Should()
            .ThrowAsync<ContentManagementSecurityException>();

        resourceProcessingServiceMock.VerifyNoOtherCalls();
        resourceEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}
