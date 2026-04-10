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
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class SubmissionEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseSubmissionAddEventAsync()
    {
        // Given
        Submission entity = CreateRandomSubmission();
        submissionEventServiceMock
            .Setup(x => x.RaiseSubmissionAddEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseSubmissionAddEventAsync(entity);

        // Then
        submissionEventServiceMock.Verify(x => x.RaiseSubmissionAddEventAsync(entity), Times.Once);
        submissionEventServiceMock.VerifyNoOtherCalls();
    }

}
















