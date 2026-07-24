// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
payload: new
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
        int actualStatusCode = await DeleteSubmissionAsync(id: createdSubmission.Id);
        int actualReadStatusCode = await GetSubmissionStatusCodeAsync(id: createdSubmission.Id);

        // Then

        actualStatusCode.Should()
            .Be(expected: 200);

        actualReadStatusCode.Should()
            .Be(expected: 404);
    }
}