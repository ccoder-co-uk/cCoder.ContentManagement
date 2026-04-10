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



using FluentAssertions;
using Moq;
using Xunit;
using CmsDataModels = cCoder.Data.Models.CMS;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class SubmissionServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForDeleteAsync()
    {
        // Given
        Guid submissionId = new Guid("11111111-1111-1111-1111-111111111111");
        Submission submission = CreateRandomSubmission(submissionId);

        submissionBrokerMock.Setup(x => x.GetAllSubmissions(false)).Returns(new[] { submission }.AsQueryable());

        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "Submission_delete"));
        submissionBrokerMock.Setup(x => x.DeleteSubmissionAsync(It.IsAny<CmsDataModels.Submission>())).ReturnsAsync(1);

        // When
        await submissionService.DeleteAsync(submissionId);

        // Then
        submissionBrokerMock.Verify(x => x.GetAllSubmissions(false), Times.Once);
        submissionBrokerMock.Verify(x => x.DeleteSubmissionAsync(It.Is<CmsDataModels.Submission>(actual => actual.Id == submission.Id)), Times.Once);
        submissionBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "Submission_delete"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        Guid submissionId = new Guid("11111111-1111-1111-1111-111111111111");
        Submission submission = CreateRandomSubmission(submissionId);

        submissionBrokerMock.Setup(x => x.GetAllSubmissions(false)).Returns(new[] { submission }.AsQueryable());

        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "Submission_delete"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await submissionService.DeleteAsync(submissionId);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        submissionBrokerMock.Verify(x => x.GetAllSubmissions(false), Times.Once);
        submissionBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "Submission_delete"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}















