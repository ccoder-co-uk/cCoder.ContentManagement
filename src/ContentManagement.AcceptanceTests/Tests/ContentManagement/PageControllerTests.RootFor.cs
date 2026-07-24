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
    public async Task RootFor_ReturnsRootPage()
    {
        // Given
        SeededPageContext seededContext = await SeedDatabase("page_create", "page_delete");
        string rootName = Unique(prefix: "RootPage");

        int rootId = (await CreatePageAsync(
payload: CreateValidPagePayload(seededContext: seededContext, name: rootName)
        )).Id;

        string childName = Unique(prefix: "ChildPage");

        int childId = (await CreatePageAsync(
payload: CreateValidPagePayload(seededContext: seededContext, name: childName, order: 2, showOnMenus: true, resourceKey: "Default", parentId: rootId)
        )).Id;

        // When
        Page actualRootPage = await GetRootPageAsync(id: childId);

        // Then

        actualRootPage.Should()
            .NotBeNull();

        actualRootPage!.Id.Should()
            .Be(expected: rootId);

        await DeletePageAsync(id: childId);
        await DeletePageAsync(id: rootId);
        await Teardown(seededContext: seededContext);
    }
}