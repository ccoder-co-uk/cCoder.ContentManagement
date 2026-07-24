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
    public async Task Post_CreatesSubmission()
    {
        // Given
        Submission expectedSubmission = new() { SourceComponent = "Acceptance" };

        // When

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

        Submission actualSubmission = await GetSubmissionAsync(id: createdSubmission.Id);

        // Then

        actualSubmission.Should()
            .NotBeNull();

        actualSubmission!.SourceComponent.Should()
            .Be(expected: expectedSubmission.SourceComponent);

        await DeleteSubmissionAsync(id: createdSubmission.Id);
    }
}