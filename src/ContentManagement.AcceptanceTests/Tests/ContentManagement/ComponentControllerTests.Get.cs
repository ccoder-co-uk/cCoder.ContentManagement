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
    public async Task Get_ReturnsCreatedComponent()
    {
        // Given
        Component expectedComponent = await CreateComponentAsync(
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

        // When
        Component actualComponent = await GetComponentAsync(id: expectedComponent.Id);

        // Then

        actualComponent.Should()
            .NotBeNull();

        actualComponent!.Id.Should()
            .Be(expected: expectedComponent.Id);

        actualComponent.Name.Should()
            .Be(expected: expectedComponent.Name);

        await DeleteComponentAsync(id: expectedComponent.Id);
    }

    [Fact]
    public async Task GetCount_ReturnsNonNegativeCount()
    {
        // Given

        // When
        int actualCount = await GetComponentCountAsync();

        // Then

        actualCount.Should()
            .BeGreaterThanOrEqualTo(expected: 0);
    }
}