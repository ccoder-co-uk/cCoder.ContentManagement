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
            new
            {
                appId = 1,
                name = Unique("Layout"),
                description = "Acceptance layout",
                headerHtml = "<title>Acceptance</title>",
                html = "<main>Acceptance layout body</main>",
                script = "console.log('layout');",
            })).Id;

        // When
        await UpdateLayoutAsync(
            id,
            new
            {
                id,
                appId = 1,
                name = Unique("LayoutUpdated"),
                description = "Updated layout",
                headerHtml = "<title>Updated</title>",
                html = "<main>Updated layout body</main>",
                script = "console.log('layout updated');",
            });

        await PatchLayoutAsync(id, new { description = "Patched layout" });

        Layout actualLayout = await GetLayoutAsync(id);

        // Then
        actualLayout.Should().NotBeNull();
        actualLayout!.Description.Should().Be("Patched layout");

        await DeleteLayoutAsync(id);
    }
}






