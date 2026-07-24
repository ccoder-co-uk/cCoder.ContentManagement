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
    public async Task Put_UpdatesResource()
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

        Resource expectedResource = new() { Description = "Updated resource" };

        // When

        await UpdateResourceAsync(
id: createdResource.Id,
payload: new
{
    id = createdResource.Id,
    appId = 1,
    name = Unique(prefix: "updatedresource")
            .ToLowerInvariant(),
    description = "Updated resource",
    key = Unique(prefix: "UpdatedKey"),
    culture = "",
    displayName = "Updated Resource",
    shortDisplayName = "Updated Resource",
});

        Resource actualResource = await GetResourceAsync(id: createdResource.Id);

        // Then

        actualResource.Should()
            .NotBeNull();

        actualResource!.Description.Should()
            .Be(expected: expectedResource.Description);

        await DeleteResourceAsync(id: createdResource.Id);
    }
}