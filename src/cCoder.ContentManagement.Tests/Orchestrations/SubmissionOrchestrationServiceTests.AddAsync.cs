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


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class SubmissionOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        Submission entity = CreateRandomSubmission();
        submissionProcessingServiceMock.Setup(x => x.AddAsync(entity)).ReturnsAsync(entity);

        submissionEventProcessingServiceMock
            .Setup(x => x.RaiseSubmissionAddEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        Submission result = await orchestrationService.AddAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        submissionProcessingServiceMock.Verify(x => x.AddAsync(entity), Times.Once);
        submissionEventProcessingServiceMock.Verify(x => x.RaiseSubmissionAddEventAsync(entity), Times.Once);
    }

}




















