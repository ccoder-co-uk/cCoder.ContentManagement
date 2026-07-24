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
    public async Task Put_UpdatesLayout()
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

        Layout expectedLayout = new() { Description = "Updated layout" };

        // When

        await UpdateLayoutAsync(
id: createdLayout.Id,
payload: new
{
    id = createdLayout.Id,
    appId = 1,
    name = Unique(prefix: "UpdatedLayout"),
    description = "Updated layout",
    headerHtml = "<title>Updated</title>",
    html = "<main>Updated layout body</main>",
    script = "console.log('layout updated');",
});

        Layout actualLayout = await GetLayoutAsync(id: createdLayout.Id);

        // Then

        actualLayout.Should()
            .NotBeNull();

        actualLayout!.Description.Should()
            .Be(expected: expectedLayout.Description);

        await DeleteLayoutAsync(id: createdLayout.Id);
    }
}