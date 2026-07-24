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
    public async Task Delete_RemovesResource()
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

        // When
        int actualStatusCode = await DeleteResourceAsync(id: createdResource.Id);
        int actualReadStatusCode = await GetResourceStatusCodeAsync(id: createdResource.Id);

        // Then

        actualStatusCode.Should()
            .Be(expected: 200);

        actualReadStatusCode.Should()
            .Be(expected: 404);
    }
}