// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using cCoder.Data.Models.CMS;
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

        submissionServiceMock.Setup(expression: x => x.GetAllSubmissions())
            .Returns(value: entities);

        // When
        IQueryable<Submission> result = submissionProcessingService.GetAllSubmissions();

        // Then

        result.Should()
            .BeSameAs(expected: entities);

        submissionServiceMock.Verify(expression: x => x.GetAllSubmissions(), times: Times.Once);
        submissionServiceMock.VerifyNoOtherCalls();
    }

}