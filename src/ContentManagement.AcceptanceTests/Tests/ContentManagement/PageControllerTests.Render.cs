using cCoder.Data.Models.CMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class PageControllerTests
{
    [Fact]
    public async Task Render_ReturnsRenderedPageContent()
    {
        // Given
        SeededPageContext seededContext = await SeedDatabase("page_create", "page_delete");

        string title = Unique("Page");
        Page createdPage = await CreatePageAsync(
            new
            {
                appId = seededContext.AppId,
                name = title,
                order = 1,
                showOnMenus = true,
                resourceKey = "Default",
                layout = seededContext.LayoutName,
                pageInfo = new[]
                {
                    new
                    {
                        cultureId = "",
                        title,
                        description = "Acceptance page",
                        keywords = "acceptance",
                    },
                },
                contents = new[]
                {
                    new
                    {
                        cultureId = "",
                        name = "body",
                        html = "<p>Acceptance page body</p>",
                    },
                },
            });

        await EnsurePageChildrenAsync(createdPage.Id, title, "Acceptance page", "acceptance", "body", "<p>Acceptance page body</p>");
        Page actualRootPage;
        MenuResponse actualMenu;
        Page actualPage;
        string actualRenderContent;

        // When
        actualRootPage = await GetRootPageAsync(createdPage.Id);
        actualMenu = await GetMenuAsync(createdPage.Id);
        actualPage = await GetPageAsync(createdPage.Id);
        actualRenderContent = await RenderPageAsync(seededContext.AppId, actualPage!.Path ?? string.Empty);

        // Then
        actualRootPage.Should().NotBeNull();
        actualRootPage!.Id.Should().Be(createdPage.Id);
        actualMenu.Success.Should().BeTrue();
        actualPage.Should().NotBeNull();
        actualPage.Path.Should().NotBeNullOrWhiteSpace();
        actualRenderContent.Should().Contain("Acceptance page body");

        await DeletePageChildrenAsync(createdPage.Id);
        await DeletePageAsync(createdPage.Id);
        await Teardown(seededContext);
    }
}






