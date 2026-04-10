using cCoder.Data.Models.CMS;
using cCoder.Data;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using System.Text.Json.Nodes;
using Xunit;


using Web.AcceptanceTests.Infrastructure;
namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class PageControllerTests
{
    [Fact]
    public async Task GetCount_ReturnsNonNegativeCount()
    {
        // Given

        // When
        int actualCount = await GetPageCountAsync();

        // Then
        actualCount.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task Get_ReturnsListOfPages()
    {
        // Given

        // When
        IReadOnlyList<Page> actualPages = await GetPagesAsync(1);

        // Then
        actualPages.Should().NotBeNull();
    }

    [Fact]
    public async Task Get_ReturnsPageById()
    {
        // Given
        SeededPageContext seededContext = await SeedDatabase(true, "page_create", "page_delete");
        string title = Unique("Page");
        Page expectedPage = await CreatePageAsync(CreateValidPagePayload(seededContext, title));
        Page actualPage;

        // When
        actualPage = await GetPageAsync(expectedPage.Id);

        // Then
        actualPage.Should().NotBeNull();
        actualPage!.Id.Should().Be(expectedPage.Id);
        actualPage.Name.Should().Be(title);

        await DeletePageAsync(expectedPage.Id);
        await Teardown(seededContext);
    }

    [Fact]
    public async Task Get_WithoutReadPrivilege_ReturnsNotFound()
    {
        SeededPageContext seededContext = await SeedDatabase(includeAppAdmin: false);

        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        Page hiddenPage = await core.AddPageAsync(new Page
        {
            AppId = seededContext.AppId,
            Name = Unique("HiddenPage"),
            Order = 1,
            ShowOnMenus = true,
            Layout = seededContext.LayoutName,
            Path = Unique("hiddenpage").ToLowerInvariant(),
            ResourceKey = "Default",
        });

        await core.AddPageInfoAsync(new PageInfo
        {
            PageId = hiddenPage.Id,
            CultureId = string.Empty,
            Title = hiddenPage.Name,
            Description = "Hidden page",
            Keywords = "hidden,page",
        });

        Page actualPage = await GetPageAsync(hiddenPage.Id);

        actualPage.Should().BeNull();

        await Teardown(seededContext);
    }

    [Fact]
    public async Task Get_WithRecursivePageExpansion_ReturnsPageInfoForNestedNavigationPages()
    {
        SeededPageContext seededContext = await SeedDatabase(true, "page_create", "page_delete");
        string rootTitle = Unique("Admin");
        string childTitle = Unique("AppManagement");
        string grandChildTitle = Unique("Settings");

        Page rootPage = await CreatePageAsync(CreateValidPagePayload(seededContext, rootTitle, order: 1));
        Page childPage = await CreatePageAsync(CreateValidPagePayload(seededContext, childTitle, order: 1, parentId: rootPage.Id));
        Page grandChildPage = await CreatePageAsync(CreateValidPagePayload(seededContext, grandChildTitle, order: 1, parentId: childPage.Id));

        JsonObject payload = await GetPageQueryPayloadAsync(
            $"?$filter=AppId eq {seededContext.AppId} and ParentId eq null&$orderby=Order asc&$expand=PageInfo,Pages($orderby=Order asc;$expand=PageInfo,Pages($orderby=Order asc;$expand=PageInfo))");

        JsonArray rootPages = payload["value"]!.AsArray();
        JsonObject adminPage = rootPages.Should().ContainSingle().Subject!.AsObject();
        JsonObject firstChildPage = adminPage["Pages"]!.AsArray().Should().ContainSingle().Subject!.AsObject();
        JsonObject nestedChildPage = firstChildPage["Pages"]!.AsArray().Should().ContainSingle().Subject!.AsObject();

        adminPage["PageInfo"]!.AsArray()[0]!["Title"]!.GetValue<string>().Should().Be(rootTitle);
        firstChildPage["PageInfo"]!.AsArray()[0]!["Title"]!.GetValue<string>().Should().Be(childTitle);
        nestedChildPage["PageInfo"]!.AsArray()[0]!["Title"]!.GetValue<string>().Should().Be(grandChildTitle);

        await DeletePageAsync(grandChildPage.Id);
        await DeletePageAsync(childPage.Id);
        await DeletePageAsync(rootPage.Id);
        await Teardown(seededContext);
    }
}







