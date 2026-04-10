using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class PageControllerTests
{
    [Fact]
    public async Task Menu_ReturnsPageMenu()
    {
        // Given
        SeededPageContext seededContext = await SeedDatabase("page_create", "page_delete");
        string title = Unique("MenuPage");
        int id = (await CreatePageAsync(CreateValidPagePayload(seededContext, title))).Id;

        // When
        MenuResponse actualMenu = await GetMenuAsync(id);

        // Then
        actualMenu.Success.Should().BeTrue();

        await Teardown(seededContext);
    }
}



