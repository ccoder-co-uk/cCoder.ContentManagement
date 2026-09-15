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
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class SubmissionOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDelegateToProcessingServiceWhenDeleteAllAsync()
    {
        // Given
        Submission[] entities = [CreateRandomSubmission()];

        submissionProcessingServiceMock.Setup(expression: x => x.DeleteAsync(submissionId: entities[0].Id))
            .Returns(value: ValueTask.CompletedTask);

        submissionEventProcessingServiceMock
            .Setup(expression: service => service.RaiseSubmissionDeleteEventAsync(
                entity: entities[0],
                userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAllSubmissionAsync(deletedSubmission: entities);

        // Then
        submissionProcessingServiceMock.Verify(
            expression: x => x.DeleteAsync(submissionId: entities[0].Id),
            times: Times.Once);

        submissionProcessingServiceMock.VerifyNoOtherCalls();

        submissionEventProcessingServiceMock.Verify(
            expression: service => service.RaiseSubmissionDeleteEventAsync(
                entity: entities[0],
                userId: CurrentUserId),
            times: Times.Once);
    }

}