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


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class SubmissionProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGetAll()
    {
        // Given
        IQueryable<Submission> entities = new[] { CreateRandomSubmission() }.AsQueryable();

        submissionServiceMock.Setup(expression: x => x.GetAllSubmission())
            .Returns(value: entities);

        // When
        IQueryable<Submission> result = submissionProcessingService.GetAllSubmission();

        // Then

        result.Should()
            .BeSameAs(expected: entities);

        submissionServiceMock.Verify(expression: x => x.GetAllSubmission(), times: Times.Once);
        submissionServiceMock.VerifyNoOtherCalls();
    }

}