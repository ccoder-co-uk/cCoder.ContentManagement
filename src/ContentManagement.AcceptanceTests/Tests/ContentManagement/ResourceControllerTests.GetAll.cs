// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class ResourceControllerTests
{
    [Fact]
    public async Task GetAll_ReturnsCreatedResource()
    {
        // Given
        string resourceName = Unique(prefix: "resource")
            .ToLowerInvariant();

        string resourceKey = Unique(prefix: "Key");

        Resource createdResource = await CreateResourceAsync(
payload: new
{
    appId = 1,
    name = resourceName,
    description = "Acceptance resource",
    key = resourceKey,
    culture = "",
    displayName = "Acceptance Resource",
    shortDisplayName = "Acceptance Resource",
});

        // When
        IReadOnlyList<Resource> actualResources = await GetAllResourcesAsync(resourceKey: resourceKey);

        // Then

        actualResources.Any(predicate: item => item.Id == createdResource.Id)
            .Should()
            .BeTrue();

        await PatchResourceAsync(id: createdResource.Id, payload: new { description = "Patched resource" });
        Resource actualResource = await GetResourceAsync(id: createdResource.Id);

        actualResource.Should()
            .NotBeNull();

        actualResource!.Description.Should()
            .Be(expected: "Patched resource");

        await DeleteResourceAsync(id: createdResource.Id);
    }
}