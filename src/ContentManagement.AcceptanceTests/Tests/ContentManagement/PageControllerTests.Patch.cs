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
    public async Task Patch_UpdatesPage()
    {
        // Given
        SeededPageContext seededContext = await SeedDatabase("page_create", "page_update", "page_delete");
        Page createdPage = await CreatePageAsync(payload: CreateValidPagePayload(seededContext: seededContext, name: Unique(prefix: "Page")));
        string updatedName = Unique(prefix: "PatchedPage");
        Page updateResponse;
        Page actualPage;

        // When

        updateResponse = await PatchPageAsync(id: createdPage.Id, payload: new
        {
            name = updatedName,
            order = 3,
        });

        actualPage = await GetPageAsync(id: createdPage.Id);

        // Then

        updateResponse.Name.Should()
            .Be(expected: updatedName);

        actualPage.Should()
            .NotBeNull();

        actualPage!.Name.Should()
            .Be(expected: updatedName);

        actualPage.Order.Should()
            .Be(expected: 3);

        await Teardown(seededContext: seededContext);
    }
}