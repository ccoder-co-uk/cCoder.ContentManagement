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
    public async Task Get_ReturnsCreatedLayout()
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

        Layout expectedLayout = new() { Id = createdLayout.Id };

        // When
        Layout actualLayout = await GetLayoutAsync(id: createdLayout.Id);

        // Then

        actualLayout.Should()
            .NotBeNull();

        actualLayout!.Id.Should()
            .Be(expected: expectedLayout.Id);

        await DeleteLayoutAsync(id: createdLayout.Id);
    }

    [Fact]
    public async Task GetCount_ReturnsNonNegativeCount()
    {
        // Given

        // When
        int actualCount = await GetLayoutCountAsync();

        // Then

        actualCount.Should()
            .BeGreaterThanOrEqualTo(expected: 0);
    }
}