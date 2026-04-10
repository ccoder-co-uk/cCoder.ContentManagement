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
    public void ShouldDelegateToFoundationServiceWhenGetAll()
    {
        // Given
        IQueryable<Submission> entities = new[] { CreateRandomSubmission() }.AsQueryable();
        submissionServiceMock.Setup(x => x.GetAll()).Returns(entities);

        // When
        IQueryable<Submission> result = submissionProcessingService.GetAll();

        // Then
        result.Should().BeSameAs(entities);
        submissionServiceMock.Verify(x => x.GetAll(), Times.Once);
        submissionServiceMock.VerifyNoOtherCalls();
    }

}
















