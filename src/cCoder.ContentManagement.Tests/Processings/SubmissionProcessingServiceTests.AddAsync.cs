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
using System.Security;



using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class SubmissionProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenAddAsync()
    {
        // Given
        Submission submission = CreateRandomSubmission();

        submissionServiceMock.Setup(expression: x => x.AddSubmissionAsync(newSubmission: submission))
            .ReturnsAsync(value: submission);

        // When
        Submission result = await submissionProcessingService.AddSubmissionAsync(newSubmission: submission);

        // Then
        Assert.Same(expected: submission, actual: result);
        submissionServiceMock.Verify(expression: x => x.AddSubmissionAsync(newSubmission: submission), times: Times.Once);
    }

    [Fact]
    public async Task ShouldPropagateSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        Submission submission = CreateRandomSubmission();

        submissionServiceMock
            .Setup(expression: x => x.AddSubmissionAsync(newSubmission: submission))
            .ThrowsAsync(exception: new SecurityException(message: "Access Denied!"));

        // When

        await Assert.ThrowsAsync<cCoder.ContentManagement.Models.Exceptions.ContentManagementSecurityException>(testCode: async () =>
            await submissionProcessingService.AddSubmissionAsync(newSubmission: submission)
        );

        // Then
    }

}