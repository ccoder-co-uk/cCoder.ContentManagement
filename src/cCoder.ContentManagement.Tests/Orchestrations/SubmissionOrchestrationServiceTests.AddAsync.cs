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
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        Submission entity = CreateRandomSubmission();

        submissionProcessingServiceMock.Setup(expression: x => x.AddSubmissionAsync(newSubmission: entity))
            .ReturnsAsync(value: entity);

        submissionEventProcessingServiceMock
            .Setup(expression: x => x.RaiseSubmissionAddEventAsync(entity: entity, userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        Submission result = await orchestrationService.AddSubmissionAsync(newSubmission: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        submissionProcessingServiceMock.Verify(expression: x => x.AddSubmissionAsync(newSubmission: entity), times: Times.Once);
        submissionEventProcessingServiceMock.Verify(expression: x => x.RaiseSubmissionAddEventAsync(entity: entity, userId: CurrentUserId), times: Times.Once);

        authorizationProcessingServiceMock.Verify(
            expression: service => service.AuthorizeAuthorizationContext(
context:                 It.Is<AuthorizationContext>(match: context =>
                    context.Request.AppId == entity.AppId
                    && context.Request.Privilege == "Submission_create")),
            times: Times.Once);

        entity.CreatedBy.Should()
            .Be(expected: CurrentUserId);

        entity.LastUpdatedBy.Should()
            .Be(expected: CurrentUserId);
    }

    [Fact]
    public async Task Submission_WhenUserLacksCreatePrivilege_IsNotPersisted()
    {
        // Given
        Submission entity = CreateRandomSubmission();

        authorizationProcessingServiceMock
            .Setup(expression: service => service.AuthorizeAuthorizationContext(
context:                 It.Is<AuthorizationContext>(match: context =>
                    context.Request.AppId == entity.AppId
                    && context.Request.Privilege == "Submission_create")))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () =>
            await orchestrationService.AddSubmissionAsync(newSubmission: entity);

        // Then
        await action.Should()
            .ThrowAsync<ContentManagementSecurityException>();

        submissionProcessingServiceMock.VerifyNoOtherCalls();
        submissionEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}