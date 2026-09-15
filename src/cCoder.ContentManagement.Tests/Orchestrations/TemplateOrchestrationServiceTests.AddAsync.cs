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
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        Template entity = CreateRandomTemplate();

        templateProcessingServiceMock.Setup(expression: x => x.AddTemplateAsync(newTemplate: entity))
            .ReturnsAsync(value: entity);

        templateEventProcessingServiceMock
            .Setup(expression: x => x.RaiseTemplateAddEventAsync(entity: entity, userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        Template result = await orchestrationService.AddTemplateAsync(newTemplate: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        templateProcessingServiceMock.Verify(expression: x => x.AddTemplateAsync(newTemplate: entity), times: Times.Once);
        templateEventProcessingServiceMock.Verify(expression: x => x.RaiseTemplateAddEventAsync(entity: entity, userId: CurrentUserId), times: Times.Once);

        authorizationProcessingServiceMock.Verify(
            expression: service => service.AuthorizeAuthorizationContext(
                context: It.Is<AuthorizationContext>(match: context =>
                    context.Request.AppId == entity.AppId
                    && context.Request.Privilege == "Template_create")),
            times: Times.Once);

        entity.CreatedBy
            .Should()
            .Be(expected: CurrentUserId);

        entity.LastUpdatedBy
            .Should()
            .Be(expected: CurrentUserId);
    }

    [Fact]
    public async Task Template_WhenUserLacksCreatePrivilege_IsNotPersisted()
    {
        // Given
        Template entity = CreateRandomTemplate();

        authorizationProcessingServiceMock
            .Setup(expression: service => service.AuthorizeAuthorizationContext(
                context: It.Is<AuthorizationContext>(match: context =>
                    context.Request.AppId == entity.AppId
                    && context.Request.Privilege == "Template_create")))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () =>
            await orchestrationService.AddTemplateAsync(newTemplate: entity);

        // Then
        await action
            .Should()
            .ThrowAsync<ContentManagementSecurityException>();

        templateProcessingServiceMock.VerifyNoOtherCalls();
        templateEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}