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

public partial class ComponentOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        Component entity = CreateRandomComponent();

        componentProcessingServiceMock.Setup(expression: x => x.AddComponentAsync(newComponent: entity))
            .ReturnsAsync(value: entity);

        componentEventProcessingServiceMock
            .Setup(expression: x => x.RaiseComponentAddEventAsync(entity: entity, userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        Component result = await orchestrationService.AddComponentAsync(newComponent: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        componentProcessingServiceMock.Verify(expression: x => x.AddComponentAsync(newComponent: entity), times: Times.Once);
        componentEventProcessingServiceMock.Verify(expression: x => x.RaiseComponentAddEventAsync(entity: entity, userId: CurrentUserId), times: Times.Once);

        authorizationProcessingServiceMock.Verify(
            expression: service => service.AuthorizeAuthorizationContext(
context:                 It.Is<AuthorizationContext>(match: context =>
                    context.Request.AppId == entity.AppId
                    && context.Request.Privilege == "Component_create")),
            times: Times.Once);

        entity.CreatedBy.Should()
            .Be(expected: CurrentUserId);

        entity.LastUpdatedBy.Should()
            .Be(expected: CurrentUserId);
    }

    [Fact]
    public async Task Component_WhenUserLacksCreatePrivilege_IsNotPersisted()
    {
        // Given
        Component entity = CreateRandomComponent();

        authorizationProcessingServiceMock
            .Setup(expression: service => service.AuthorizeAuthorizationContext(
context:                 It.Is<AuthorizationContext>(match: context =>
                    context.Request.AppId == entity.AppId
                    && context.Request.Privilege == "Component_create")))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () =>
            await orchestrationService.AddComponentAsync(newComponent: entity);

        // Then
        await action.Should()
            .ThrowAsync<ContentManagementSecurityException>();

        componentProcessingServiceMock.VerifyNoOtherCalls();
        componentEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}