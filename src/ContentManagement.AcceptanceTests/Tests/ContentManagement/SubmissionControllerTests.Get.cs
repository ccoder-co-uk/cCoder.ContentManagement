using cCoder.Data.Models.CMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class SubmissionControllerTests
{
    [Fact]
    public async Task GetCount_ReturnsNonNegativeCount()
    {
        // Given

        // When
        int actualCount = await GetSubmissionCountAsync();

        // Then
        actualCount.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task Get_ReturnsListOfSubmissions()
    {
        // Given

        // When
        IReadOnlyList<Submission> actualSubmissions = await GetSubmissionsAsync();

        // Then
        actualSubmissions.Should().NotBeNull();
    }
}





