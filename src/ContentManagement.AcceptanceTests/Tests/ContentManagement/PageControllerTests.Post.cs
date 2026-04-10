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
}






