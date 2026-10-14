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

public sealed partial class TemplateControllerTests
{
    [Fact]
    public async Task Get_ReturnsCreatedTemplate()
    {
        // Given
        Template createdTemplate = await CreateTemplateAsync(
payload: new
{
    appId = 1,
    name = Unique(prefix: "Template"),
    description = "Acceptance template",
    resourceKey = "Default",
    rawString = "<html><body><h1>[model[title]]</h1></body></html>",
});

        Template expectedTemplate = new() { Id = createdTemplate.Id };

        // When
        Template actualTemplate = await GetTemplateAsync(id: createdTemplate.Id);

        // Then

        actualTemplate.Should()
            .NotBeNull();

        actualTemplate!.Id.Should()
            .Be(expected: expectedTemplate.Id);

        await DeleteTemplateAsync(id: createdTemplate.Id);
    }

    [Fact]
    public async Task GetCount_ReturnsNonNegativeCount()
    {
        // Given

        // When
        int actualCount = await GetTemplateCountAsync();

        // Then

        actualCount.Should()
            .BeGreaterThanOrEqualTo(expected: 0);
    }

    [Fact]
    public async Task Get_WithAppExpansion_ReturnsApp()
    {
        // Given
        Template createdTemplate = await CreateTemplateAsync(
payload: new
{
    appId = 1,
    name = Unique(prefix: "ExpansionTemplate"),
    description = "Expansion template",
    resourceKey = "Default",
    rawString = "<html><body>Expansion</body></html>",
});

        // When
        using var response = await Client.GetAsync(
requestUri: $"{BaseUrl}({createdTemplate.Id})?$expand=App");

        string content = await response.Content.ReadAsStringAsync();

        // Then
        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        JsonObject actualTemplate = JsonNode.Parse(json: content)!.AsObject();

        actualTemplate["App"]!.AsObject()["Id"]!.GetValue<int>()
            .Should()
            .Be(expected: 1);

        await DeleteTemplateAsync(id: createdTemplate.Id);
    }
}