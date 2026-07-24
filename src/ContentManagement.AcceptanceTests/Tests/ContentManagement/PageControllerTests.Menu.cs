// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using Xunit;

namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class PageControllerTests
{
    [Fact]
    public async Task Menu_ReturnsPageMenu()
    {
        // Given
        SeededPageContext seededContext = await SeedDatabase(privileges: ["page_create", "page_delete"]);
        string title = Unique(prefix: "MenuPage");
        int id = (await CreatePageAsync(payload: CreateValidPagePayload(seededContext: seededContext, name: title))).Id;

        // When
        MenuResponse actualMenu = await GetMenuAsync(id: id);

        // Then
        actualMenu.Success.Should()
            .BeTrue();

        await Teardown(seededContext: seededContext);
    }
}