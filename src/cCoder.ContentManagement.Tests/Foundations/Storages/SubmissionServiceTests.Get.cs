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


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class SubmissionServiceTests
{
    [Fact]
    public void ShouldReturnSubmissionWhenGet()
    {
        // Given
        Guid submissionId = new Guid("11111111-1111-1111-1111-111111111111");
        Submission submission = CreateRandomSubmission(submissionId);

        submissionBrokerMock.Setup(x => x.GetAllSubmissions(false)).Returns(new[] { submission }.AsQueryable());

        // When
        Submission result = submissionService.Get(submissionId);

        // Then
        result.Should().BeEquivalentTo(submission);
        submissionBrokerMock.Verify(x => x.GetAllSubmissions(false), Times.Once);
        submissionBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}


















