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
    public async Task Put_UpdatesComponent()
    {
        // Given
        Component createdComponent = await CreateComponentAsync(
payload: new
{
    appId = 1,
    name = Unique(prefix: "Component"),
    description = "Acceptance component",
    resourceKey = "Default",
    content = "<div>Hello component</div>",
    script = "console.log('component');",
    key = "Acceptance",
});

        Component actualComponent;

        // When

        await UpdateComponentAsync(
id: createdComponent.Id,
payload: new
{
    id = createdComponent.Id,
    appId = 1,
    name = Unique(prefix: "UpdatedComponent"),
    description = "Updated component",
    resourceKey = "Default",
    content = "<div>Hello updated component</div>",
    script = "console.log('updated component');",
    key = "Acceptance",
});

        actualComponent = await GetComponentAsync(id: createdComponent.Id);

        // Then

        actualComponent.Should()
            .NotBeNull();

        actualComponent!.Description.Should()
            .Be(expected: "Updated component");

        await DeleteComponentAsync(id: createdComponent.Id);
    }
}