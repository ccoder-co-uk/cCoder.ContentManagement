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
    public async Task Delete_RemovesPage()
    {
        // Given
        SeededPageContext seededContext = await SeedDatabase("page_create", "page_delete");
        Page createdPage = await CreatePageAsync(payload: CreateValidPagePayload(seededContext: seededContext, name: Unique(prefix: "Page")));
        int actualReadStatusCode;

        // When
        int actualStatusCode = await DeletePageAsync(id: createdPage.Id);
        actualReadStatusCode = await GetPageStatusCodeAsync(id: createdPage.Id);

        // Then

        actualStatusCode.Should()
            .Be(expected: 200);

        actualReadStatusCode.Should()
            .Be(expected: 404);

        await Teardown(seededContext: seededContext);
    }
}