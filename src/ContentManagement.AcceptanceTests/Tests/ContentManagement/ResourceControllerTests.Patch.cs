using cCoder.Data.Models.CMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class ResourceControllerTests
{
    [Fact]
    public async Task Patch_UpdatesResourceDescription()
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
        Resource expectedResource = new() { Description = "Patched resource" };

        // When
        await PatchResourceAsync(createdResource.Id, new { description = "Patched resource" });
        Resource actualResource = await GetResourceAsync(createdResource.Id);

        // Then
        actualResource.Should().NotBeNull();
        actualResource!.Description.Should().Be(expectedResource.Description);

        await DeleteResourceAsync(createdResource.Id);
    }
}






