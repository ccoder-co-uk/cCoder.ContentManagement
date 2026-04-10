using cCoder.Data.Models.CMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class SubmissionControllerTests
{
    [Fact]
    public async Task Post_CreatesSubmission()
    {
        // Given
        Submission expectedSubmission = new() { SourceComponent = "Acceptance" };

        // When
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
        Submission actualSubmission = await GetSubmissionAsync(createdSubmission.Id);

        // Then
        actualSubmission.Should().NotBeNull();
        actualSubmission!.SourceComponent.Should().Be(expectedSubmission.SourceComponent);

        await DeleteSubmissionAsync(createdSubmission.Id);
    }
}






