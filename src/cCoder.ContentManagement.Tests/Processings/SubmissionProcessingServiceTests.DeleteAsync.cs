// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class SubmissionProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenDeleteAsync()
    {
        // Given
        Submission entity = CreateRandomSubmission();
        var id = entity.Id;

        submissionServiceMock.Setup(expression: x => x.DeleteAsync(submissionId: id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await submissionProcessingService.DeleteAsync(submissionId: id);

        // Then
        submissionServiceMock.Verify(expression: x => x.DeleteAsync(submissionId: id), times: Times.Once);
        submissionServiceMock.VerifyNoOtherCalls();
    }

}