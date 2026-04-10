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
            new
            {
                appId = 1,
                name = Unique("Layout"),
                description = "Acceptance layout",
                headerHtml = "<title>Acceptance</title>",
                html = "<main>Acceptance layout body</main>",
                script = "console.log('layout');",
            });
        Layout expectedLayout = new() { Description = "Updated layout" };

        // When
        await UpdateLayoutAsync(
            createdLayout.Id,
            new
            {
                id = createdLayout.Id,
                appId = 1,
                name = Unique("UpdatedLayout"),
                description = "Updated layout",
                headerHtml = "<title>Updated</title>",
                html = "<main>Updated layout body</main>",
                script = "console.log('layout updated');",
            });
        Layout actualLayout = await GetLayoutAsync(createdLayout.Id);

        // Then
        actualLayout.Should().NotBeNull();
        actualLayout!.Description.Should().Be(expectedLayout.Description);

        await DeleteLayoutAsync(createdLayout.Id);
    }
}






