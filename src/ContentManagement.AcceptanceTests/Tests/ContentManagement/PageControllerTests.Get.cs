// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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

        actualCount.Should()
            .BeGreaterThanOrEqualTo(expected: 0);
    }

    [Fact]
    public async Task Get_ReturnsListOfPages()
    {
        // Given

        // When
        IReadOnlyList<Page> actualPages = await GetPagesAsync(top: 1);

        // Then

        actualPages.Should()
            .NotBeNull();
    }

    [Fact]
    public async Task Get_ReturnsPageById()
    {
        // Given
        SeededPageContext seededContext = await SeedDatabase(
            includeAppAdmin: true,
            privileges: ["page_create", "page_delete"]);
        string title = Unique(prefix: "Page");
        Page expectedPage = await CreatePageAsync(payload: CreateValidPagePayload(seededContext: seededContext, name: title));
        Page actualPage;

        // When
        actualPage = await GetPageAsync(id: expectedPage.Id);

        // Then

        actualPage.Should()
            .NotBeNull();

        actualPage!.Id.Should()
            .Be(expected: expectedPage.Id);

        actualPage.Name.Should()
            .Be(expected: title);

        await DeletePageAsync(id: expectedPage.Id);
        await Teardown(seededContext: seededContext);
    }

    [Fact]
    public async Task Get_WithoutReadPrivilege_ReturnsNotFound()
    {
        SeededPageContext seededContext = await SeedDatabase(includeAppAdmin: false);

        using IServiceScope scope = fixture.Factory.Services.CreateScope();

        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        Page hiddenPage = await core.AddPageAsync(page: new Page
        {
            AppId = seededContext.AppId,
            Name = Unique(prefix: "HiddenPage"),
            Order = 1,
            ShowOnMenus = true,
            Layout = seededContext.LayoutName,
            Path = Unique(prefix: "hiddenpage")
            .ToLowerInvariant(),
            ResourceKey = "Default",
        });

        await core.AddPageInfoAsync(pageInfo: new PageInfo
        {
            PageId = hiddenPage.Id,
            CultureId = string.Empty,
            Title = hiddenPage.Name,
            Description = "Hidden page",
            Keywords = "hidden,page",
        });

        Page actualPage = await GetPageAsync(id: hiddenPage.Id);

        actualPage.Should()
            .BeNull();

        await Teardown(seededContext: seededContext);
    }

    [Fact]
    public async Task Get_WithRecursivePageExpansion_ReturnsPageInfoForNestedNavigationPages()
    {
        SeededPageContext seededContext = await SeedDatabase(
            includeAppAdmin: true,
            privileges: ["page_create", "page_delete"]);
        string rootTitle = Unique(prefix: "Admin");
        string childTitle = Unique(prefix: "AppManagement");
        string grandChildTitle = Unique(prefix: "Settings");

        Page rootPage = await CreatePageAsync(payload: CreateValidPagePayload(seededContext: seededContext, name: rootTitle, order: 1));
        Page childPage = await CreatePageAsync(payload: CreateValidPagePayload(seededContext: seededContext, name: childTitle, order: 1, parentId: rootPage.Id));
        Page grandChildPage = await CreatePageAsync(payload: CreateValidPagePayload(seededContext: seededContext, name: grandChildTitle, order: 1, parentId: childPage.Id));

        JsonObject payload = await GetPageQueryPayloadAsync(
queryString: $"?$filter=AppId eq {seededContext.AppId} and ParentId eq null&$orderby=Order asc&$expand=PageInfo,Pages($orderby=Order asc;$expand=PageInfo,Pages($orderby=Order asc;$expand=PageInfo))");

        JsonArray rootPages = payload["value"]!.AsArray();

        JsonObject adminPage = rootPages.Should()
            .ContainSingle()
            .Subject!.AsObject();

        JsonObject firstChildPage = adminPage["Pages"]!.AsArray()
            .Should()
            .ContainSingle()
            .Subject!.AsObject();

        JsonObject nestedChildPage = firstChildPage["Pages"]!.AsArray()
            .Should()
            .ContainSingle()
            .Subject!.AsObject();

        adminPage["PageInfo"]!.AsArray()[0]!["Title"]!.GetValue<string>()
            .Should()
            .Be(expected: rootTitle);

        firstChildPage["PageInfo"]!.AsArray()[0]!["Title"]!.GetValue<string>()
            .Should()
            .Be(expected: childTitle);

        nestedChildPage["PageInfo"]!.AsArray()[0]!["Title"]!.GetValue<string>()
            .Should()
            .Be(expected: grandChildTitle);

        await DeletePageAsync(id: grandChildPage.Id);
        await DeletePageAsync(id: childPage.Id);
        await DeletePageAsync(id: rootPage.Id);
        await Teardown(seededContext: seededContext);
    }
}
