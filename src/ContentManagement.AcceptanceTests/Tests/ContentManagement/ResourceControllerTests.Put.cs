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
            new
            {
                appId = 1,
                name = Unique("resource").ToLowerInvariant(),
                description = "Acceptance resource",
                key = Unique("Key"),
                culture = "",
                displayName = "Acceptance Resource",
                shortDisplayName = "Acceptance Resource",
            });
        Resource expectedResource = new() { Description = "Updated resource" };

        // When
        await UpdateResourceAsync(
            createdResource.Id,
            new
            {
                id = createdResource.Id,
                appId = 1,
                name = Unique("updatedresource").ToLowerInvariant(),
                description = "Updated resource",
                key = Unique("UpdatedKey"),
                culture = "",
                displayName = "Updated Resource",
                shortDisplayName = "Updated Resource",
            });
        Resource actualResource = await GetResourceAsync(createdResource.Id);

        // Then
        actualResource.Should().NotBeNull();
        actualResource!.Description.Should().Be(expectedResource.Description);

        await DeleteResourceAsync(createdResource.Id);
    }
}






