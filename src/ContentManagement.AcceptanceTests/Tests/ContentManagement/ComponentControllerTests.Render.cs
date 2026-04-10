using cCoder.Data.Models.CMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class ComponentControllerTests
{
    [Fact]
    public async Task Render_ReturnsRenderedComponentMarkup()
    {
        // Given
        string componentName = Unique("Component");
        Component createdComponent = await CreateComponentAsync(
            new
            {
                appId = 1,
                name = componentName,
                description = "Acceptance component",
                resourceKey = "Default",
                content = "<div>Hello component</div>",
                script = "console.log('component');",
                key = "Acceptance",
            });
        string actualRenderContent;
        Component actualComponent;

        // When
        actualRenderContent = await RenderComponentAsync(1, componentName);

        // Then
        actualRenderContent.Should().Contain(componentName);
        actualRenderContent.Should().Contain("Hello component");

        await PatchComponentAsync(createdComponent.Id, new { description = "Patched component" });
        actualComponent = await GetComponentAsync(createdComponent.Id);

        actualComponent.Should().NotBeNull();
        actualComponent!.Description.Should().Be("Patched component");

        await DeleteComponentAsync(createdComponent.Id);
    }
}






