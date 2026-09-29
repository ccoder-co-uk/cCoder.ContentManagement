// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class SubmissionProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGet()
    {
        // Given
        Submission entity = CreateRandomSubmission();
        var id = entity.Id;

        submissionServiceMock.Setup(expression: x => x.GetSubmission(submissionId: id))
            .Returns(value: entity);

        // When
        Submission result = submissionProcessingService.GetSubmission(submissionId: id);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        submissionServiceMock.Verify(expression: x => x.GetSubmission(submissionId: id), times: Times.Once);
        submissionServiceMock.VerifyNoOtherCalls();
    }

}