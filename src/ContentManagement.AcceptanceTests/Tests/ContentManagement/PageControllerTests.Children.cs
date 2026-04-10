using cCoder.Data.Models.CMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class PageControllerTests
{
    [Fact]
    public async Task Put_UpdatesGivenDefaultCulturePageInfo()
    {
        // Given
        SeededPageContext seededContext = await SeedDatabase("page_create", "page_update");
        Page createdPage = await CreatePageAsync(CreateValidPagePayload(seededContext, Unique("Page")));
        string updatedTitle = Unique("UpdatedTitle");

        // When
        _ = await UpdatePageAsync(createdPage.Id, new
        {
            id = createdPage.Id,
            appId = seededContext.AppId,
            name = createdPage.Name,
            order = createdPage.Order,
            showOnMenus = createdPage.ShowOnMenus,
            resourceKey = createdPage.ResourceKey,
            layout = seededContext.LayoutName,
            pageInfo = new[]
            {
                new
                {
                    cultureId = "",
                    title = updatedTitle,
                    description = "Updated description",
                    keywords = "updated,keywords",
                },
            },
        });

        PageInfo[] actualPageInfos = GetPageInfos(createdPage.Id);

        // Then
        actualPageInfos.Should().ContainSingle();
        actualPageInfos[0].CultureId.Should().Be(string.Empty);
        actualPageInfos[0].Title.Should().Be(updatedTitle);
        actualPageInfos[0].Description.Should().Be("Updated description");
        actualPageInfos[0].Keywords.Should().Be("updated,keywords");

        await Teardown(seededContext);
    }

    [Fact]
    public async Task Put_RecomputesChildPathsWhenParentNameChanges()
    {
        // Given
        SeededPageContext seededContext = await SeedDatabase("page_create", "page_update");
        Page parentPage = await CreatePageAsync(CreateValidPagePayload(seededContext, Unique("ParentPage")));
        Page childPage = await CreatePageAsync(
            CreateValidPagePayload(seededContext, Unique("ChildPage"), parentId: parentPage.Id)
        );
        string updatedParentName = Unique("RenamedParent");

        // When
        _ = await UpdatePageAsync(parentPage.Id, new
        {
            id = parentPage.Id,
            appId = seededContext.AppId,
            name = updatedParentName,
            order = parentPage.Order,
            showOnMenus = parentPage.ShowOnMenus,
            resourceKey = parentPage.ResourceKey,
            layout = seededContext.LayoutName,
            pageInfo = new[]
            {
                new
                {
                    cultureId = "",
                    title = updatedParentName,
                    description = $"{updatedParentName} description",
                    keywords = "parent,updated",
                },
            },
        });

        Page actualParent = await GetPageAsync(parentPage.Id);
        Page actualChild = await GetPageAsync(childPage.Id);

        // Then
        actualParent.Should().NotBeNull();
        actualParent!.Name.Should().Be(updatedParentName);
        actualParent.Path.Should().Be(updatedParentName);

        actualChild.Should().NotBeNull();
        actualChild!.ParentId.Should().Be(parentPage.Id);
        actualChild.Path.Should().StartWith($"{updatedParentName}/");

        await Teardown(seededContext);
    }
}






