using cCoder.Data.Models.CMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class SubmissionControllerTests
{
    [Fact]
    public async Task Patch_UpdatesSubmissionState()
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
        Submission expectedSubmission = new() { State = "Patched" };

        // When
        await PatchSubmissionAsync(createdSubmission.Id, new { state = "Patched" });
        Submission actualSubmission = await GetSubmissionAsync(createdSubmission.Id);

        // Then
        actualSubmission.Should().NotBeNull();
        actualSubmission!.State.Should().Be(expectedSubmission.State);

        await DeleteSubmissionAsync(createdSubmission.Id);
    }
}






