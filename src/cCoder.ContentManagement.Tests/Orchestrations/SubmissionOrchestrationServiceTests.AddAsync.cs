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
            .Setup(expression: x => x.RaiseSubmissionAddEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        Submission result = await orchestrationService.AddSubmissionAsync(newSubmission: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        submissionProcessingServiceMock.Verify(expression: x => x.AddSubmissionAsync(newSubmission: entity), times: Times.Once);
        submissionEventProcessingServiceMock.Verify(expression: x => x.RaiseSubmissionAddEventAsync(entity: entity), times: Times.Once);
    }

}