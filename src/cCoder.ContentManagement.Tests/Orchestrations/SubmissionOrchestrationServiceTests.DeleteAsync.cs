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
    public async Task ShouldGetThenDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        // Given
        Guid id = Guid.NewGuid();
        Submission entity = CreateRandomSubmission();

        submissionProcessingServiceMock.Setup(expression: x => x.GetSubmission(submissionId: id))
            .Returns(value: entity);

        submissionProcessingServiceMock.Setup(expression: x => x.DeleteAsync(submissionId: id))
            .Returns(value: ValueTask.CompletedTask);

        submissionEventProcessingServiceMock
            .Setup(expression: x => x.RaiseSubmissionDeleteEventAsync(entity: entity, userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(submissionId: id);

        // Then
        submissionProcessingServiceMock.Verify(expression: x => x.GetSubmission(submissionId: id), times: Times.Once);
        submissionProcessingServiceMock.Verify(expression: x => x.DeleteAsync(submissionId: id), times: Times.Once);
        submissionEventProcessingServiceMock.Verify(expression: x => x.RaiseSubmissionDeleteEventAsync(entity: entity, userId: CurrentUserId), times: Times.Once);

        authorizationProcessingServiceMock.Verify(
            expression: service => service.AuthorizeAuthorizationContext(
                It.Is<AuthorizationContext>(context =>
                    context.Request.AppId == entity.AppId
                    && context.Request.Privilege == "Submission_delete")),
            times: Times.Once);
    }

    [Fact]
    public async Task Submission_WhenUserLacksDeletePrivilege_IsNotDeleted()
    {
        // Given
        Guid id = Guid.NewGuid();
        Submission entity = CreateRandomSubmission();

        submissionProcessingServiceMock
            .Setup(expression: service => service.GetSubmission(submissionId: id))
            .Returns(value: entity);

        authorizationProcessingServiceMock
            .Setup(expression: service => service.AuthorizeAuthorizationContext(
                It.Is<AuthorizationContext>(context =>
                    context.Request.AppId == entity.AppId
                    && context.Request.Privilege == "Submission_delete")))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () =>
            await orchestrationService.DeleteAsync(submissionId: id);

        // Then
        await action.Should()
            .ThrowAsync<ContentManagementSecurityException>();

        submissionProcessingServiceMock.Verify(
            expression: service => service.GetSubmission(submissionId: id),
            times: Times.Once);

        submissionProcessingServiceMock.VerifyNoOtherCalls();
        submissionEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}