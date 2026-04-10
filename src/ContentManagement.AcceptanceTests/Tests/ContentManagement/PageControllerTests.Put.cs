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
        SeededPageContext seededContext = await SeedDatabase("page_create", "page_update", "page_delete");
        Page createdPage = await CreatePageAsync(CreateValidPagePayload(seededContext, Unique("Page")));
        string updatedName = Unique("UpdatedPage");
        Page updateResponse;
        Page actualPage;

        // When
        updateResponse = await UpdatePageAsync(createdPage.Id, new
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

        actualPage = await GetPageAsync(createdPage.Id);

        // Then
        updateResponse.Name.Should().Be(updatedName);
        actualPage.Should().NotBeNull();
        actualPage!.Name.Should().Be(updatedName);
        actualPage.Order.Should().Be(2);

        await Teardown(seededContext);
    }
}






