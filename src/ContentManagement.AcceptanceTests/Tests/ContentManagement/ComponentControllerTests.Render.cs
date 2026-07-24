// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        string componentName = Unique(prefix: "Component");

        Component createdComponent = await CreateComponentAsync(
payload: new
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
        actualRenderContent = await RenderComponentAsync(appId: 1, name: componentName);

        // Then

        actualRenderContent.Should()
            .Contain(expected: componentName);

        actualRenderContent.Should()
            .Contain(expected: "Hello component");

        await PatchComponentAsync(id: createdComponent.Id, payload: new { description = "Patched component" });
        actualComponent = await GetComponentAsync(id: createdComponent.Id);

        actualComponent.Should()
            .NotBeNull();

        actualComponent!.Description.Should()
            .Be(expected: "Patched component");

        await DeleteComponentAsync(id: createdComponent.Id);
    }
}