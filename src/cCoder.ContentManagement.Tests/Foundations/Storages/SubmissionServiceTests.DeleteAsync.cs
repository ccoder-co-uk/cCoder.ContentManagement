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
using System.Security;



using FluentAssertions;
using Moq;
using Xunit;
using CmsDataModels = cCoder.Data.Models.CMS;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class SubmissionServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenDeleteAsync()
    {
        // Given
        Guid submissionId = new Guid(g: "11111111-1111-1111-1111-111111111111");
        Submission submission = CreateRandomSubmission(id: submissionId);

        submissionBrokerMock.Setup(expression: x => x.GetAllSubmissions())
            .Returns(value: new[] { submission }.AsQueryable());
submissionBrokerMock.Setup(expression: x => x.DeleteSubmissionAsync(deletedSubmission: It.IsAny<CmsDataModels.Submission>()))
            .ReturnsAsync(value: 1);

        // When
        await submissionService.DeleteAsync(submissionId: submissionId);

        // Then
        submissionBrokerMock.Verify(expression: x => x.GetAllSubmissions(), times: Times.Once);
        submissionBrokerMock.Verify(expression: x => x.DeleteSubmissionAsync(deletedSubmission: It.Is<CmsDataModels.Submission>(match: actual => actual.Id == submission.Id)), times: Times.Once);
        submissionBrokerMock.VerifyNoOtherCalls();
}
}