// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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

        string title = Unique(prefix: "Page");

        Page createdPage = await CreatePageAsync(
payload: new
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

        await EnsurePageChildrenAsync(pageId: createdPage.Id, title: title, description: "Acceptance page", keywords: "acceptance", contentName: "body", html: "<p>Acceptance page body</p>");
        Page actualRootPage;
        MenuResponse actualMenu;
        Page actualPage;
        string actualRenderContent;

        // When
        actualRootPage = await GetRootPageAsync(id: createdPage.Id);
        actualMenu = await GetMenuAsync(id: createdPage.Id);
        actualPage = await GetPageAsync(id: createdPage.Id);
        actualRenderContent = await RenderPageAsync(appId: seededContext.AppId, path: actualPage!.Path ?? string.Empty);

        // Then

        actualRootPage.Should()
            .NotBeNull();

        actualRootPage!.Id.Should()
            .Be(expected: createdPage.Id);

        actualMenu.Success.Should()
            .BeTrue();

        actualPage.Should()
            .NotBeNull();

        actualPage.Path.Should()
            .NotBeNullOrWhiteSpace();

        actualRenderContent.Should()
            .Contain(expected: "Acceptance page body");

        await DeletePageChildrenAsync(pageId: createdPage.Id);
        await DeletePageAsync(id: createdPage.Id);
        await Teardown(seededContext: seededContext);
    }
}