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
    public async Task Put_UpdatesGivenDefaultCulturePageInfo()
    {
        // Given
        SeededPageContext seededContext = await SeedDatabase(privileges: ["page_create", "page_update"]);
        Page createdPage = await CreatePageAsync(payload: CreateValidPagePayload(seededContext: seededContext, name: Unique(prefix: "Page")));
        string updatedTitle = Unique(prefix: "UpdatedTitle");

        // When
        _ = await UpdatePageAsync(id: createdPage.Id, payload: new
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

        PageInfo[] actualPageInfos = GetPageInfos(pageId: createdPage.Id);

        // Then
        actualPageInfos.Should()
            .ContainSingle();

        actualPageInfos[0].CultureId.Should()
            .Be(expected: string.Empty);

        actualPageInfos[0].Title.Should()
            .Be(expected: updatedTitle);

        actualPageInfos[0].Description.Should()
            .Be(expected: "Updated description");

        actualPageInfos[0].Keywords.Should()
            .Be(expected: "updated,keywords");

        await Teardown(seededContext: seededContext);
    }

    [Fact]
    public async Task Put_RecomputesChildPathsWhenParentNameChanges()
    {
        // Given
        SeededPageContext seededContext = await SeedDatabase(privileges: ["page_create", "page_update"]);
        Page parentPage = await CreatePageAsync(payload: CreateValidPagePayload(seededContext: seededContext, name: Unique(prefix: "ParentPage")));

        Page childPage = await CreatePageAsync(
payload: CreateValidPagePayload(
    seededContext: seededContext,
    name: Unique(prefix: "ChildPage"),
    parentId: parentPage.Id)
        );

        string updatedParentName = Unique(prefix: "RenamedParent");

        // When
        _ = await UpdatePageAsync(id: parentPage.Id, payload: new
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

        Page actualParent = await GetPageAsync(id: parentPage.Id);
        Page actualChild = await GetPageAsync(id: childPage.Id);

        // Then
        actualParent.Should()
            .NotBeNull();

        actualParent!.Name.Should()
            .Be(expected: updatedParentName);

        actualParent.Path.Should()
            .Be(expected: updatedParentName);

        actualChild.Should()
            .NotBeNull();

        actualChild!.ParentId.Should()
            .Be(expected: parentPage.Id);

        actualChild.Path.Should()
            .StartWith(expected: $"{updatedParentName}/");

        await Teardown(seededContext: seededContext);
    }
}