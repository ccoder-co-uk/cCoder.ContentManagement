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

public sealed partial class ComponentControllerTests
{
    [Fact]
    public async Task Get_ReturnsCreatedComponent()
    {
        // Given
        Component expectedComponent = await CreateComponentAsync(
payload: new
{
    appId = 1,
    name = Unique(prefix: "Component"),
    description = "Acceptance component",
    resourceKey = "Default",
    content = "<div>Hello component</div>",
    script = "console.log('component');",
    key = "Acceptance",
});

        // When
        Component actualComponent = await GetComponentAsync(id: expectedComponent.Id);

        // Then

        actualComponent.Should()
            .NotBeNull();

        actualComponent!.Id.Should()
            .Be(expected: expectedComponent.Id);

        actualComponent.Name.Should()
            .Be(expected: expectedComponent.Name);

        await DeleteComponentAsync(id: expectedComponent.Id);
    }

    [Fact]
    public async Task GetCount_ReturnsNonNegativeCount()
    {
        // Given

        // When
        int actualCount = await GetComponentCountAsync();

        // Then

        actualCount.Should()
            .BeGreaterThanOrEqualTo(expected: 0);
    }

    [Fact]
    public async Task Get_WithAppExpansion_ReturnsApp()
    {
        // Given
        Component createdComponent = await CreateComponentAsync(
payload: new
{
    appId = 1,
    name = Unique(prefix: "Component"),
    description = "Expansion component",
    resourceKey = "Default",
    content = "<div>Expansion component</div>",
    script = "console.log('component');",
    key = "Expansion",
});

        // When
        using var response = await Client.GetAsync(
requestUri: $"{BaseUrl}({createdComponent.Id})?$expand=App");

        string content = await response.Content.ReadAsStringAsync();

        // Then
        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        JsonObject actualComponent = JsonNode.Parse(json: content)!.AsObject();

        actualComponent["App"]!.AsObject()["Id"]!.GetValue<int>()
            .Should()
            .Be(expected: 1);

        await DeleteComponentAsync(id: createdComponent.Id);
    }
}