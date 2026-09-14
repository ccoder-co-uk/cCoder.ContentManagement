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

public partial class SubmissionOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseUpdateEventAsyncWhenUpdateAsync()
    {
        // Given
        Submission entity = CreateRandomSubmission();

        submissionProcessingServiceMock.Setup(expression: x => x.UpdateSubmissionAsync(updatedSubmission: entity))
            .ReturnsAsync(value: entity);

        submissionEventProcessingServiceMock
            .Setup(expression: x => x.RaiseSubmissionUpdateEventAsync(entity: entity, userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        Submission result = await orchestrationService.UpdateSubmissionAsync(updatedSubmission: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        submissionProcessingServiceMock.Verify(expression: x => x.UpdateSubmissionAsync(updatedSubmission: entity), times: Times.Once);
        submissionEventProcessingServiceMock.Verify(expression: x => x.RaiseSubmissionUpdateEventAsync(entity: entity, userId: CurrentUserId), times: Times.Once);

        authorizationProcessingServiceMock.Verify(
            expression: service => service.AuthorizeAuthorizationContext(
                It.Is<AuthorizationContext>(context =>
                    context.Request.AppId == entity.AppId
                    && context.Request.Privilege == "Submission_update")),
            times: Times.Once);

        entity.LastUpdatedBy.Should().Be(CurrentUserId);
    }

    [Fact]
    public async Task Submission_WhenUserLacksUpdatePrivilege_IsNotPersisted()
    {
        // Given
        Submission entity = CreateRandomSubmission();

        authorizationProcessingServiceMock
            .Setup(expression: service => service.AuthorizeAuthorizationContext(
                It.Is<AuthorizationContext>(context =>
                    context.Request.AppId == entity.AppId
                    && context.Request.Privilege == "Submission_update")))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () =>
            await orchestrationService.UpdateSubmissionAsync(updatedSubmission: entity);

        // Then
        await action.Should()
            .ThrowAsync<ContentManagementSecurityException>();

        submissionProcessingServiceMock.VerifyNoOtherCalls();
        submissionEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}