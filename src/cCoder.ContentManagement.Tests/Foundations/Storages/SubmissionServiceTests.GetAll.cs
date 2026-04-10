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
using CmsDataModels = cCoder.Data.Models.CMS;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class SubmissionServiceTests
{
    [Fact]
    public void ShouldReturnSubmissionsWhenGetAll()
    {
        // Given
        Guid submissionId = new Guid("11111111-1111-1111-1111-111111111111");
        Submission[] expectedItems =
        {
            CreateRandomSubmission(submissionId),
        };
        IQueryable<CmsDataModels.Submission> submissions = expectedItems
            .Select(item => item)
            .AsQueryable();

        submissionBrokerMock.Setup(x => x.GetAllSubmissions(false)).Returns(submissions);

        // When
        IQueryable<Submission> result = submissionService.GetAll();

        // Then
        result.Should().BeEquivalentTo(expectedItems);
        submissionBrokerMock.Verify(x => x.GetAllSubmissions(false), Times.Once);
        submissionBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}

















