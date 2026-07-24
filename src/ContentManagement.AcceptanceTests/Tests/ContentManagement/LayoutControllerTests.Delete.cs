// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class LayoutControllerTests
{
    [Fact]
    public async Task Delete_RemovesLayout()
    {
        // Given
        Layout createdLayout = await CreateLayoutAsync(
payload: new
{
    appId = 1,
    name = Unique(prefix: "Layout"),
    description = "Acceptance layout",
    headerHtml = "<title>Acceptance</title>",
    html = "<main>Acceptance layout body</main>",
    script = "console.log('layout');",
});

        // When
        int actualStatusCode = await DeleteLayoutAsync(id: createdLayout.Id);
        int actualReadStatusCode = await GetLayoutStatusCodeAsync(id: createdLayout.Id);

        // Then

        actualStatusCode.Should()
            .Be(expected: 200);

        actualReadStatusCode.Should()
            .Be(expected: 404);
    }
}