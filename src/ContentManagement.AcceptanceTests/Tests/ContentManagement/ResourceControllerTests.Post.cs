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
    public async Task Post_CreatesResource()
    {
        // Given
        string name = Unique(prefix: "resource")
            .ToLowerInvariant();

        Resource expectedResource = new() { Name = name };

        // When

        Resource createdResource = await CreateResourceAsync(
payload: new
{
    appId = 1,
    name,
    description = "Acceptance resource",
    key = Unique(prefix: "Key"),
    culture = "",
    displayName = "Acceptance Resource",
    shortDisplayName = "Acceptance Resource",
});

        Resource actualResource = await GetResourceAsync(id: createdResource.Id);

        // Then

        actualResource.Should()
            .NotBeNull();

        actualResource!.Name.Should()
            .Be(expected: expectedResource.Name);

        await DeleteResourceAsync(id: createdResource.Id);
    }
}