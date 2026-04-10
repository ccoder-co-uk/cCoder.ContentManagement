using cCoder.Data.Models.CMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class ComponentControllerTests
{
    [Fact]
    public async Task Put_UpdatesComponent()
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
        await UpdateComponentAsync(
            createdComponent.Id,
            new
            {
                id = createdComponent.Id,
                appId = 1,
                name = Unique("UpdatedComponent"),
                description = "Updated component",
                resourceKey = "Default",
                content = "<div>Hello updated component</div>",
                script = "console.log('updated component');",
                key = "Acceptance",
            });

        actualComponent = await GetComponentAsync(createdComponent.Id);

        // Then
        actualComponent.Should().NotBeNull();
        actualComponent!.Description.Should().Be("Updated component");

        await DeleteComponentAsync(createdComponent.Id);
    }
}






