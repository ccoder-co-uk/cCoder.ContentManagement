using cCoder.Data.Models.CMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class SubmissionControllerTests
{
    [Fact]
    public async Task Delete_RemovesSubmission()
    {
        // Given
        Submission createdSubmission = await CreateSubmissionAsync(
            new
            {
                appId = 1,
                createdBy = "Guest",
                lastUpdatedBy = "Guest",
                createdOn = DateTimeOffset.UtcNow,
                lastUpdatedOn = DateTimeOffset.UtcNow,
                sourceComponent = "Acceptance",
                state = "New",
                dataJson = "{\"name\":\"Acceptance\"}",
            });

        // When
        int actualStatusCode = await DeleteSubmissionAsync(createdSubmission.Id);
        int actualReadStatusCode = await GetSubmissionStatusCodeAsync(createdSubmission.Id);

        // Then
        actualStatusCode.Should().Be(200);
        actualReadStatusCode.Should().Be(404);
    }
}





