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
    public async Task Delete_RemovesComponent()
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

        int actualReadStatusCode;

        // When
        int actualStatusCode = await DeleteComponentAsync(id: createdComponent.Id);
        actualReadStatusCode = await GetComponentStatusCodeAsync(id: createdComponent.Id);

        // Then

        actualStatusCode.Should()
            .Be(expected: 204);

        actualReadStatusCode.Should()
            .Be(expected: 404);
    }
}