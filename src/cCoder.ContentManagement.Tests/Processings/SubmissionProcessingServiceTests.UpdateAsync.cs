// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Security;
using ComponentRenderParams = cCoder.ContentManagement.Models.ComponentRenderParams;
using Config = cCoder.ContentManagement.Models.Config;
using PageRenderParams = cCoder.ContentManagement.Models.PageRenderParams;
using PageRoleInfo = cCoder.ContentManagement.Models.PageRoleInfo;
using RenderParams = cCoder.ContentManagement.Models.RenderParams;
using RenderResult = cCoder.ContentManagement.Models.RenderResult;
using TemplateRenderParams = cCoder.ContentManagement.Models.TemplateRenderParams;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class SubmissionProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenUpdateAsync()
    {
        // Given
        Submission entity = CreateRandomSubmission();

        submissionServiceMock.Setup(expression: x => x.UpdateSubmissionAsync(updatedSubmission: entity))
            .ReturnsAsync(value: entity);

        // When
        Submission result = await submissionProcessingService.UpdateSubmissionAsync(updatedSubmission: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        submissionServiceMock.Verify(expression: x => x.UpdateSubmissionAsync(updatedSubmission: entity), times: Times.Once);
        submissionServiceMock.VerifyNoOtherCalls();
    }

}