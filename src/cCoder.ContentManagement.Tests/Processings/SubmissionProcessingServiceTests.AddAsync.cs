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
        submissionServiceMock.Setup(x => x.AddAsync(submission)).ReturnsAsync(submission);

        // When
        Submission result = await submissionProcessingService.AddAsync(submission);

        // Then
        Assert.Same(submission, result);
        submissionServiceMock.Verify(x => x.AddAsync(submission), Times.Once);
    }

    [Fact]
    public async Task ShouldPropagateSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        Submission submission = CreateRandomSubmission();

        submissionServiceMock
            .Setup(x => x.AddAsync(submission))
            .ThrowsAsync(new SecurityException("Access Denied!"));

        // When
        await Assert.ThrowsAsync<SecurityException>(async () =>
            await submissionProcessingService.AddAsync(submission)
        );

        // Then
    }

}














