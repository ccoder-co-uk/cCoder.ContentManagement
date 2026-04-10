using cCoder.Data.Models.CMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class ComponentControllerTests
{
    [Fact]
    public async Task Get_ReturnsCreatedComponent()
    {
        // Given
        Component expectedComponent = await CreateComponentAsync(
            new
            {
                appId = 1,
                name = Unique("Component"),
                description = "Acceptance component",
                resourceKey = "Default",
                content = "<div>Hello component</div>",
                script = "console.log('component');",
                key = "Acceptance",
            });

        // When
        Component actualComponent = await GetComponentAsync(expectedComponent.Id);

        // Then
        actualComponent.Should().NotBeNull();
        actualComponent!.Id.Should().Be(expectedComponent.Id);
        actualComponent.Name.Should().Be(expectedComponent.Name);

        await DeleteComponentAsync(expectedComponent.Id);
    }

    [Fact]
    public async Task GetCount_ReturnsNonNegativeCount()
    {
        // Given

        // When
        int actualCount = await GetComponentCountAsync();

        // Then
        actualCount.Should().BeGreaterThanOrEqualTo(0);
    }
}






