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
    public async Task Get_ReturnsCreatedResource()
    {
        // Given
        Resource createdResource = await CreateResourceAsync(
payload: new
{
    appId = 1,
    name = Unique(prefix: "resource")
            .ToLowerInvariant(),
    description = "Acceptance resource",
    key = Unique(prefix: "Key"),
    culture = "",
    displayName = "Acceptance Resource",
    shortDisplayName = "Acceptance Resource",
});

        Resource expectedResource = new() { Id = createdResource.Id };

        // When
        Resource actualResource = await GetResourceAsync(id: createdResource.Id);

        // Then

        actualResource.Should()
            .NotBeNull();

        actualResource!.Id.Should()
            .Be(expected: expectedResource.Id);

        await DeleteResourceAsync(id: createdResource.Id);
    }

    [Fact]
    public async Task GetCount_ReturnsNonNegativeCount()
    {
        // Given

        // When
        int actualCount = await GetResourceCountAsync();

        // Then

        actualCount.Should()
            .BeGreaterThanOrEqualTo(expected: 0);
    }
}