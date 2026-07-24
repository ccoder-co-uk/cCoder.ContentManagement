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
    public async Task Patch_UpdatesLayoutDescription()
    {
        // Given
        int id = (await CreateLayoutAsync(
payload: new
{
    appId = 1,
    name = Unique(prefix: "Layout"),
    description = "Acceptance layout",
    headerHtml = "<title>Acceptance</title>",
    html = "<main>Acceptance layout body</main>",
    script = "console.log('layout');",
})).Id;

        // When

        await UpdateLayoutAsync(
id: id,
payload: new
{
    id,
    appId = 1,
    name = Unique(prefix: "LayoutUpdated"),
    description = "Updated layout",
    headerHtml = "<title>Updated</title>",
    html = "<main>Updated layout body</main>",
    script = "console.log('layout updated');",
});

        await PatchLayoutAsync(id: id, payload: new { description = "Patched layout" });

        Layout actualLayout = await GetLayoutAsync(id: id);

        // Then

        actualLayout.Should()
            .NotBeNull();

        actualLayout!.Description.Should()
            .Be(expected: "Patched layout");

        await DeleteLayoutAsync(id: id);
    }
}