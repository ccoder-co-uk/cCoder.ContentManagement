// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
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

        actualCount.Should()
            .BeGreaterThanOrEqualTo(expected: 0);
    }

    [Fact]
    public async Task Get_ReturnsListOfSubmissions()
    {
        // Given

        // When
        IReadOnlyList<Submission> actualSubmissions = await GetSubmissionsAsync();

        // Then

        actualSubmissions.Should()
            .NotBeNull();
    }

    [Fact]
    public async Task Get_WithAppExpansion_ReturnsApp()
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
    sourceComponent = "Expansion",
    state = "New",
    dataJson = "{\"name\":\"Expansion\"}",
});

        // When
        using var response = await Client.GetAsync(
requestUri: $"{BaseUrl}({createdSubmission.Id})?$expand=App");

        string content = await response.Content.ReadAsStringAsync();

        // Then
        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        JsonObject actualSubmission = JsonNode.Parse(json: content)!.AsObject();

        actualSubmission["App"]!.AsObject()["Id"]!.GetValue<int>()
            .Should()
            .Be(expected: 1);

        await DeleteSubmissionAsync(id: createdSubmission.Id);
    }
}