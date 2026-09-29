// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Linq;
using cCoder.Data.Models.CMS;
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
        Guid submissionId = new Guid(g: "11111111-1111-1111-1111-111111111111");
        Submission submission = CreateRandomSubmission(id: submissionId);

        submissionBrokerMock.Setup(expression: x => x.GetAllSubmissions())
            .Returns(value: new[] { submission }.AsQueryable());

        // When
        Submission result = submissionService.GetSubmission(submissionId: submissionId);

        // Then

        result.Should()
            .BeEquivalentTo(expectation: submission);

        submissionBrokerMock.Verify(expression: x => x.GetAllSubmissions(), times: Times.Once);
        submissionBrokerMock.VerifyNoOtherCalls();
}

}