// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using System.Security;



using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class SubmissionProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenAddAsync()
    {
        // Given
        Submission submission = CreateRandomSubmission();

        submissionServiceMock.Setup(expression: x => x.AddSubmissionAsync(newSubmission: submission))
            .ReturnsAsync(value: submission);

        // When
        Submission result = await submissionProcessingService.AddSubmissionAsync(newSubmission: submission);

        // Then
        Assert.Same(expected: submission, actual: result);
        submissionServiceMock.Verify(expression: x => x.AddSubmissionAsync(newSubmission: submission), times: Times.Once);
    }

    [Fact]
    public async Task ShouldPropagateSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        Submission submission = CreateRandomSubmission();

        submissionServiceMock
            .Setup(expression: x => x.AddSubmissionAsync(newSubmission: submission))
            .ThrowsAsync(exception: new SecurityException(message: "Access Denied!"));

        // When

        await Assert.ThrowsAsync<cCoder.ContentManagement.Models.Exceptions.ContentManagementSecurityException>(testCode: async () =>
            await submissionProcessingService.AddSubmissionAsync(newSubmission: submission)
        );

        // Then
    }

}