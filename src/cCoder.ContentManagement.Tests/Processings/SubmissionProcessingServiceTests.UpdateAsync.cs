// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class SubmissionProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenUpdateAsync()
    {
        // Given
        Submission entity = CreateRandomSubmission();

        submissionServiceMock.Setup(expression: x => x.UpdateSubmissionAsync(updatedSubmission: entity))
            .ReturnsAsync(value: entity);

        // When
        Submission result = await submissionProcessingService.UpdateSubmissionAsync(updatedSubmission: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        submissionServiceMock.Verify(expression: x => x.UpdateSubmissionAsync(updatedSubmission: entity), times: Times.Once);
        submissionServiceMock.VerifyNoOtherCalls();
    }

}