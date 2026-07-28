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
            .Setup(expression: x => x.RaiseSubmissionDeleteEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(submissionId: id);

        // Then
        submissionProcessingServiceMock.Verify(expression: x => x.GetSubmission(submissionId: id), times: Times.Once);
        submissionProcessingServiceMock.Verify(expression: x => x.DeleteAsync(submissionId: id), times: Times.Once);
        submissionEventProcessingServiceMock.Verify(expression: x => x.RaiseSubmissionDeleteEventAsync(entity: entity), times: Times.Once);
    }

}