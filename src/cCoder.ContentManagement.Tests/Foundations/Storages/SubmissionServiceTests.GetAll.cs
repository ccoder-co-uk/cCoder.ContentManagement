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
        Guid submissionId = new Guid(g: "11111111-1111-1111-1111-111111111111");

        Submission[] expectedItems =
        {
            CreateRandomSubmission(id: submissionId),
        };

        IQueryable<CmsDataModels.Submission> submissions = expectedItems
            .Select(selector: item => item)
            .AsQueryable();

        submissionBrokerMock.Setup(expression: x => x.GetAllSubmissions())
            .Returns(value: submissions);

        // When
        IQueryable<Submission> result = submissionService.GetAllSubmission();

        // Then

        result.Should()
            .BeEquivalentTo(expectation: expectedItems);

        submissionBrokerMock.Verify(expression: x => x.GetAllSubmissions(), times: Times.Once);
        submissionBrokerMock.VerifyNoOtherCalls();
        authorizationManagerMock.VerifyNoOtherCalls();
    }

}