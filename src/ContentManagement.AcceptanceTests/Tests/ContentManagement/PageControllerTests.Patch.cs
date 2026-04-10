using cCoder.Data.Models.CMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class PageControllerTests
{
    [Fact]
    public async Task Patch_UpdatesPage()
    {
        // Given
        SeededPageContext seededContext = await SeedDatabase("page_create", "page_update", "page_delete");
        Page createdPage = await CreatePageAsync(CreateValidPagePayload(seededContext, Unique("Page")));
        string updatedName = Unique("PatchedPage");
        Page updateResponse;
        Page actualPage;

        // When
        updateResponse = await PatchPageAsync(createdPage.Id, new
        {
            name = updatedName,
            order = 3,
        });

        actualPage = await GetPageAsync(createdPage.Id);

        // Then
        updateResponse.Name.Should().Be(updatedName);
        actualPage.Should().NotBeNull();
        actualPage!.Name.Should().Be(updatedName);
        actualPage.Order.Should().Be(3);

        await Teardown(seededContext);
    }
}






