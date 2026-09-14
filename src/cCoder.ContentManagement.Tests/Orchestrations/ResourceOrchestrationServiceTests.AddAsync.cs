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
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        Resource entity = CreateRandomResource();

        resourceProcessingServiceMock.Setup(expression: x => x.AddResourceAsync(newResource: entity))
            .ReturnsAsync(value: entity);

        resourceEventProcessingServiceMock
            .Setup(expression: x => x.RaiseResourceAddEventAsync(entity: entity, userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        Resource result = await orchestrationService.AddResourceAsync(newResource: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        resourceProcessingServiceMock.Verify(expression: x => x.AddResourceAsync(newResource: entity), times: Times.Once);
        resourceEventProcessingServiceMock.Verify(expression: x => x.RaiseResourceAddEventAsync(entity: entity, userId: CurrentUserId), times: Times.Once);

        authorizationProcessingServiceMock.Verify(
            expression: service => service.AuthorizeAuthorizationContext(
                It.Is<AuthorizationContext>(context =>
                    context.Request.AppId == entity.AppId
                    && context.Request.Privilege == "Resource_create")),
            times: Times.Once);

        entity.CreatedBy.Should().Be(CurrentUserId);
        entity.LastUpdatedBy.Should().Be(CurrentUserId);
    }

    [Fact]
    public async Task Resource_WhenUserLacksCreatePrivilege_IsNotPersisted()
    {
        // Given
        Resource entity = CreateRandomResource();

        authorizationProcessingServiceMock
            .Setup(expression: service => service.AuthorizeAuthorizationContext(
                It.Is<AuthorizationContext>(context =>
                    context.Request.AppId == entity.AppId
                    && context.Request.Privilege == "Resource_create")))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () =>
            await orchestrationService.AddResourceAsync(newResource: entity);

        // Then
        await action.Should()
            .ThrowAsync<ContentManagementSecurityException>();

        resourceProcessingServiceMock.VerifyNoOtherCalls();
        resourceEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}
