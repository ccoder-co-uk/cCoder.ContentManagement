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
            new
            {
                appId = 1,
                name = Unique("Layout"),
                description = "Acceptance layout",
                headerHtml = "<title>Acceptance</title>",
                html = "<main>Acceptance layout body</main>",
                script = "console.log('layout');",
            });

        // When
        int actualStatusCode = await DeleteLayoutAsync(createdLayout.Id);
        int actualReadStatusCode = await GetLayoutStatusCodeAsync(createdLayout.Id);

        // Then
        actualStatusCode.Should().Be(200);
        actualReadStatusCode.Should().Be(404);
    }
}





