using cCoder.Data.Models.CMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class ComponentControllerTests
{
    [Fact]
    public async Task Patch_UpdatesComponentDescription()
    {
        // Given
        Component createdComponent = await CreateComponentAsync(
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
        Component actualComponent;

        // When
        await PatchComponentAsync(createdComponent.Id, new { description = "Patched component" });
        actualComponent = await GetComponentAsync(createdComponent.Id);

        // Then
        actualComponent.Should().NotBeNull();
        actualComponent!.Description.Should().Be("Patched component");

        await DeleteComponentAsync(createdComponent.Id);
    }
}






