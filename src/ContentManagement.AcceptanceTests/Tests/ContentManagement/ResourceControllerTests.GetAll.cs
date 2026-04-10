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
        string resourceName = Unique("resource").ToLowerInvariant();
        string resourceKey = Unique("Key");
        Resource createdResource = await CreateResourceAsync(
            new
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
        IReadOnlyList<Resource> actualResources = await GetAllResourcesAsync(resourceKey);

        // Then
        actualResources.Any(item => item.Id == createdResource.Id).Should().BeTrue();

        await PatchResourceAsync(createdResource.Id, new { description = "Patched resource" });
        Resource actualResource = await GetResourceAsync(createdResource.Id);

        actualResource.Should().NotBeNull();
        actualResource!.Description.Should().Be("Patched resource");

        await DeleteResourceAsync(createdResource.Id);
    }
}






