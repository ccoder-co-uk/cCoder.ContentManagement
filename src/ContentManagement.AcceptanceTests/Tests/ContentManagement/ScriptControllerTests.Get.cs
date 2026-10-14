// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class ScriptControllerTests
{
    [Fact]
    public async Task Get_ReturnsCreatedScript()
    {
        // Given
        Script createdScript = await CreateScriptAsync(
payload: new
{
    appId = 1,
    name = Unique(prefix: "Script"),
    description = "Acceptance script",
    key = "Acceptance",
    content = "return 42;",
});

        Script expectedScript = new() { Id = createdScript.Id };

        // When
        Script actualScript = await GetScriptAsync(id: createdScript.Id);

        // Then

        actualScript.Should()
            .NotBeNull();

        actualScript!.Id.Should()
            .Be(expected: expectedScript.Id);

        await DeleteScriptAsync(id: createdScript.Id);
    }

    [Fact]
    public async Task GetCount_ReturnsNonNegativeCount()
    {
        // Given

        // When
        int actualCount = await GetScriptCountAsync();

        // Then

        actualCount.Should()
            .BeGreaterThanOrEqualTo(expected: 0);
    }

    [Fact]
    public async Task Get_WithAppExpansion_ReturnsApp()
    {
        // Given
        Script createdScript = await CreateScriptAsync(
payload: new
{
    appId = 1,
    name = Unique(prefix: "ExpansionScript"),
    description = "Expansion script",
    key = "Expansion",
    content = "return 42;",
});

        // When
        using var response = await Client.GetAsync(
requestUri: $"{BaseUrl}({createdScript.Id})?$expand=App");

        string content = await response.Content.ReadAsStringAsync();

        // Then
        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        JsonObject actualScript = JsonNode.Parse(json: content)!.AsObject();

        actualScript["App"]!.AsObject()["Id"]!.GetValue<int>()
            .Should()
            .Be(expected: 1);

        await DeleteScriptAsync(id: createdScript.Id);
    }
}