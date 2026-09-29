// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class SubmissionEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseSubmissionDeleteEventAsync()
    {
        // Given
        Submission entity = CreateRandomSubmission();

        submissionEventServiceMock
            .Setup(expression: x => x.RaiseSubmissionDeleteEventAsync(
                entity: entity,
                userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseSubmissionDeleteEventAsync(
            submission: entity,
            userId: CurrentUserId);

        // Then
        submissionEventServiceMock.Verify(expression: x => x.RaiseSubmissionDeleteEventAsync(
            entity: entity,
            userId: CurrentUserId), times: Times.Once);

        submissionEventServiceMock.VerifyNoOtherCalls();
    }

}