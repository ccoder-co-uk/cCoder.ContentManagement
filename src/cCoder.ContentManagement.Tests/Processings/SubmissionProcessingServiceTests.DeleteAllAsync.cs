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


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class SubmissionProcessingServiceTests
{
    [Fact]
    public async Task ShouldUseFoundationDeleteAsyncPerItemWhenDeleteAllAsync()
    {
        // Given
        Submission entity = CreateRandomSubmission();
        var id = entity.Id;

        submissionServiceMock.Setup(expression: x => x.DeleteAsync(submissionId: id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await submissionProcessingService.DeleteAllSubmissionAsync(deletedSubmission: new[] { entity });

        // Then
        submissionServiceMock.Verify(expression: x => x.DeleteAsync(submissionId: id), times: Times.Once);
        submissionServiceMock.VerifyNoOtherCalls();
    }

}