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

    [Fact]
    public async Task Render_WhenComponentContainsNestedComponent_RendersUntilStable()
    {
        // Given
        string childName = Unique(prefix: "ChildComponent");
        string parentName = Unique(prefix: "ParentComponent");

        Component childComponent = await CreateComponentAsync(
            payload: new
            {
                appId = 1,
                name = childName,
                description = "Nested acceptance component",
                resourceKey = "Default",
                content = "<strong>Nested acceptance marker</strong>",
                script = string.Empty,
                key = "Acceptance",
            });

        Component parentComponent = await CreateComponentAsync(
            payload: new
            {
                appId = 1,
                name = parentName,
                description = "Parent acceptance component",
                resourceKey = "Default",
                content = $"<div>[component[{childName}]]</div>",
                script = string.Empty,
                key = "Acceptance",
            });

        // When
        string actualRenderContent = await RenderComponentAsync(
            name: parentName);

        // Then
        actualRenderContent.Should()
            .Contain(expected: "Nested acceptance marker");

        actualRenderContent.Should()
            .NotContain(unexpected: "[component[");

        actualRenderContent.Should()
            .NotContain(unexpected: "[[render-fragment:");

        await DeleteComponentAsync(id: parentComponent.Id);
        await DeleteComponentAsync(id: childComponent.Id);
    }
}