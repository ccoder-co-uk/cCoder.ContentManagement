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
        // When
        actualRenderContent = await RenderComponentAsync(name: componentName);

        // Then

        actualRenderContent.Should()
            .Contain(expected: componentName);

        actualRenderContent.Should()
            .Contain(expected: "Hello component");

        await DeleteComponentAsync(id: createdComponent.Id);
    }
}