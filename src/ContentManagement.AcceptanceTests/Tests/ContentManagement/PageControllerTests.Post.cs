// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using System.Net.Http.Json;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Xunit;

namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class PageControllerTests
{
    [Fact]
    public async Task Post_CreatesPage()
    {
        // Given
        SeededPageContext seededContext = await SeedDatabase(privileges: ["page_create", "page_delete"]);
        string name = Unique(prefix: "Page");
        Page expectedPage;
        Page actualPage;

        // When
        expectedPage = await CreatePageAsync(payload: CreateValidPagePayload(seededContext: seededContext, name: name));

        actualPage = await GetPageAsync(id: expectedPage.Id);

        // Then
        actualPage.Should()
            .NotBeNull();

        actualPage!.Name.Should()
            .Be(expected: name);

        await DeletePageAsync(id: expectedPage.Id);
        await Teardown(seededContext: seededContext);
    }

    [Fact]
    public async Task Post_WhenComputedPathAlreadyExistsForSameApp_ShouldReturnDuplicatePathError()
    {
        // Given
        SeededPageContext seededContext = await SeedDatabase(privileges: ["page_create", "page_delete"]);
        string name = Unique(prefix: "Page");
        Page existingPage = await CreatePageAsync(payload: CreateValidPagePayload(seededContext: seededContext, name: name));

        // When
        using HttpResponseMessage response = await Client.PostAsJsonAsync(
requestUri: BaseUrl,
value: CreateValidPagePayload(seededContext: seededContext, name: name));

        string content = await response.Content.ReadAsStringAsync();

        // Then
        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.BadRequest, because: content);

        content.Should()
            .NotContain(unexpected: "could not be translated");

        await DeletePageAsync(id: existingPage.Id);
        await Teardown(seededContext: seededContext);
    }

    [Fact]
    public async Task Post_WhenLayoutDoesNotExistForApp_ShouldReturnLayoutError()
    {
        // Given
        SeededPageContext seededContext = await SeedDatabase(privileges: "page_create");
        string name = Unique(prefix: "Page");
        string missingLayout = Unique(prefix: "MissingLayout");

        // When
        using HttpResponseMessage response = await Client.PostAsJsonAsync(
requestUri: BaseUrl,
value: new
{
    appId = seededContext.AppId,
    name,
    order = 1,
    showOnMenus = true,
    resourceKey = "Default",
    layout = missingLayout,
    pageInfo = new[]
                {
                    new
                    {
                        cultureId = "",
                        title = name,
                        description = $"{name} description",
                        keywords = $"{name.ToLowerInvariant()},acceptance",
                    },
                },
    contents = new[]
                {
                    new
                    {
                        cultureId = "",
                        name = "body",
                        html = $"<p>{name} body</p>",
                    },
                },
});

        string content = await response.Content.ReadAsStringAsync();

        // Then
        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.BadRequest, because: content);

        await Teardown(seededContext: seededContext);
    }
}