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
        submissionServiceMock.Setup(x => x.UpdateAsync(entity)).ReturnsAsync(entity);

        // When
        Submission result = await submissionProcessingService.UpdateAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        submissionServiceMock.Verify(x => x.UpdateAsync(entity), Times.Once);
        submissionServiceMock.VerifyNoOtherCalls();
    }

}
















