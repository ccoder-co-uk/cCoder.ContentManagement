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
        SeededPageContext seededContext = await SeedDatabase("page_create", "page_delete");
        string name = Unique("Page");
        Page expectedPage;
        Page actualPage;

        // When
        expectedPage = await CreatePageAsync(CreateValidPagePayload(seededContext, name));

        actualPage = await GetPageAsync(expectedPage.Id);

        // Then
        actualPage.Should().NotBeNull();
        actualPage!.Name.Should().Be(name);

        await DeletePageAsync(expectedPage.Id);
        await Teardown(seededContext);
    }

    [Fact]
    public async Task Post_WhenComputedPathAlreadyExistsForSameApp_ShouldReturnDuplicatePathError()
    {
        // Given
        SeededPageContext seededContext = await SeedDatabase("page_create", "page_delete");
        string name = Unique("Page");
        Page existingPage = await CreatePageAsync(CreateValidPagePayload(seededContext, name));

        // When
        using HttpResponseMessage response = await Client.PostAsJsonAsync(
            BaseUrl,
            CreateValidPagePayload(seededContext, name));
        string content = await response.Content.ReadAsStringAsync();

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError, content);
        content.Should().Contain($"A page already exists for app {seededContext.AppId} with path '{name}'.");
        content.Should().NotContain("could not be translated");

        await DeletePageAsync(existingPage.Id);
        await Teardown(seededContext);
    }
}






