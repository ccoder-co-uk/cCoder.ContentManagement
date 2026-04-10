using cCoder.Data.Models.CMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class LayoutControllerTests
{
    [Fact]
    public async Task Post_CreatesLayout()
    {
        // Given
        string name = Unique("Layout");
        Layout expectedLayout = new() { Name = name };

        // When
        Layout createdLayout = await CreateLayoutAsync(
            new
            {
                appId = 1,
                name,
                description = "Acceptance layout",
                headerHtml = "<title>Acceptance</title>",
                html = "<main>Acceptance layout body</main>",
                script = "console.log('layout');",
            });
        Layout actualLayout = await GetLayoutAsync(createdLayout.Id);

        // Then
        actualLayout.Should().NotBeNull();
        actualLayout!.Name.Should().Be(expectedLayout.Name);

        await DeleteLayoutAsync(createdLayout.Id);
    }
}






