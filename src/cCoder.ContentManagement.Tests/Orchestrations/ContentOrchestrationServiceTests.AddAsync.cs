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

public partial class ContentOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        Content entity = CreateRandomContent();

        contentProcessingServiceMock.Setup(expression: x => x.AddContentAsync(newContent: entity))
            .ReturnsAsync(value: entity);

        contentEventProcessingServiceMock
            .Setup(expression: x => x.RaiseContentAddEventAsync(entity: entity, userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        Content result = await orchestrationService.AddContentAsync(newContent: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        contentProcessingServiceMock.Verify(expression: x => x.AddContentAsync(newContent: entity), times: Times.Once);
        contentEventProcessingServiceMock.Verify(expression: x => x.RaiseContentAddEventAsync(entity: entity, userId: CurrentUserId), times: Times.Once);
    }

    [Fact]
    public async Task Content_WhenUserLacksCreatePrivilege_IsNotPersisted()
    {
        // Given
        Content entity = CreateRandomContent();
        entity.Page = new Page { AppId = 73 };

        authorizationProcessingServiceMock
            .Setup(expression: service => service.AuthorizeAuthorizationContext(
                It.Is<AuthorizationContext>(context =>
                    context.Request.AppId == entity.Page.AppId
                    && context.Request.Privilege == "Content_create")))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () =>
            await orchestrationService.AddContentAsync(newContent: entity);

        // Then
        await action.Should()
            .ThrowAsync<ContentManagementSecurityException>();

        contentProcessingServiceMock.VerifyNoOtherCalls();
        contentEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}
