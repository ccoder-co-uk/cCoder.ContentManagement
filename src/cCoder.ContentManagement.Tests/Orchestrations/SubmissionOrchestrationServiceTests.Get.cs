// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class SubmissionOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultWhenGet()
    {
        // Given
        Guid id = Guid.NewGuid();
        Submission entity = CreateRandomSubmission();

        submissionProcessingServiceMock.Setup(expression: x => x.GetSubmission(submissionId: id))
            .Returns(value: entity);

        // When
        Submission result = orchestrationService.GetSubmission(submissionId: id);

        // Then

        result.Should()
            .BeEquivalentTo(expectation: entity, config: options => options.Excluding(expression: submission => submission.Data));

        submissionProcessingServiceMock.Verify(expression: x => x.GetSubmission(submissionId: id), times: Times.Once);
        submissionProcessingServiceMock.VerifyNoOtherCalls();
        submissionEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}