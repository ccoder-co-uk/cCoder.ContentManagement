using cCoder.Data.Models.CMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class ComponentControllerTests
{
    [Fact]
    public async Task Post_CreatesComponent()
    {
        // Given
        string name = Unique("Component");
        Component expectedComponent;

        // When
        expectedComponent = await CreateComponentAsync(
            new
            {
                appId = 1,
                name,
                description = "Acceptance component",
                resourceKey = "Default",
                content = "<div>Hello component</div>",
                script = "console.log('component');",
                key = "Acceptance",
            });

        Component actualComponent = await GetComponentAsync(expectedComponent.Id);

        // Then
        actualComponent.Should().NotBeNull();
        actualComponent!.Name.Should().Be(name);

        await DeleteComponentAsync(expectedComponent.Id);
    }
}






