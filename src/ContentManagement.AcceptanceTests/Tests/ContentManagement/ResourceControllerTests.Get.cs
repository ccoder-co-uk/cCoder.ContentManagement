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
        Resource expectedResource = new() { Id = createdResource.Id };

        // When
        Resource actualResource = await GetResourceAsync(createdResource.Id);

        // Then
        actualResource.Should().NotBeNull();
        actualResource!.Id.Should().Be(expectedResource.Id);

        await DeleteResourceAsync(createdResource.Id);
    }

    [Fact]
    public async Task GetCount_ReturnsNonNegativeCount()
    {
        // Given

        // When
        int actualCount = await GetResourceCountAsync();

        // Then
        actualCount.Should().BeGreaterThanOrEqualTo(0);
    }
}






