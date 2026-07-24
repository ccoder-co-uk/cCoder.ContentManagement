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
    public async Task Patch_UpdatesComponentDescription()
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
        await PatchComponentAsync(id: createdComponent.Id, payload: new { description = "Patched component" });
        actualComponent = await GetComponentAsync(id: createdComponent.Id);

        // Then

        actualComponent.Should()
            .NotBeNull();

        actualComponent!.Description.Should()
            .Be(expected: "Patched component");

        await DeleteComponentAsync(id: createdComponent.Id);
    }
}