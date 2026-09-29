// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Linq;
using cCoder.Data.Models.CMS;
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
}

}