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
    public async Task Put_UpdatesPage()
    {
        // Given
        SeededPageContext seededContext = await SeedDatabase(privileges: ["page_create", "page_update", "page_delete"]);
        Page createdPage = await CreatePageAsync(payload: CreateValidPagePayload(seededContext: seededContext, name: Unique(prefix: "Page")));
        string updatedName = Unique(prefix: "UpdatedPage");
        Page updateResponse;
        Page actualPage;

        // When
        updateResponse = await UpdatePageAsync(id: createdPage.Id, payload: new
        {
            id = createdPage.Id,
            appId = seededContext.AppId,
            name = updatedName,
            order = 2,
            showOnMenus = true,
            resourceKey = "Default",
            layout = seededContext.LayoutName,
            pageInfo = new[]
            {
                new
                {
                    cultureId = "",
                    title = updatedName,
                    description = "Updated page description",
                    keywords = "updated,acceptance",
                },
            },
            contents = new[]
            {
                new
                {
                    cultureId = "",
                    name = "body",
                    html = "<p>Updated page body</p>",
                },
            },
        });

        actualPage = await GetPageAsync(id: createdPage.Id);

        // Then
        updateResponse.Name.Should()
            .Be(expected: updatedName);

        actualPage.Should()
            .NotBeNull();

        actualPage!.Name.Should()
            .Be(expected: updatedName);

        actualPage.Order.Should()
            .Be(expected: 2);

        await Teardown(seededContext: seededContext);
    }

    [Fact]
    public async Task Put_WhenLayoutDoesNotExistForApp_ShouldReturnLayoutError()
    {
        // Given
        SeededPageContext seededContext = await SeedDatabase(privileges: ["page_create", "page_update", "page_delete"]);
        Page createdPage = await CreatePageAsync(payload: CreateValidPagePayload(seededContext: seededContext, name: Unique(prefix: "Page")));
        string updatedName = Unique(prefix: "UpdatedPage");
        string missingLayout = Unique(prefix: "MissingLayout");

        // When
        using HttpResponseMessage response = await Client.PutAsJsonAsync(requestUri: $"{BaseUrl}({createdPage.Id})", value: new
        {
            id = createdPage.Id,
            appId = seededContext.AppId,
            name = updatedName,
            order = 2,
            showOnMenus = true,
            resourceKey = "Default",
            layout = missingLayout,
            pageInfo = new[]
            {
                new
                {
                    cultureId = "",
                    title = updatedName,
                    description = "Updated page description",
                    keywords = "updated,acceptance",
                },
            },
            contents = new[]
            {
                new
                {
                    cultureId = "",
                    name = "body",
                    html = "<p>Updated page body</p>",
                },
            },
        });

        string content = await response.Content.ReadAsStringAsync();

        // Then
        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.InternalServerError, because: content);

        content.Should()
            .Contain(expected: $"Layout '{missingLayout}' does not exist for app {seededContext.AppId}.");

        await Teardown(seededContext: seededContext);
    }
}