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
        string name = Unique("resource").ToLowerInvariant();
        Resource expectedResource = new() { Name = name };

        // When
        Resource createdResource = await CreateResourceAsync(
            new
            {
                appId = 1,
                name,
                description = "Acceptance resource",
                key = Unique("Key"),
                culture = "",
                displayName = "Acceptance Resource",
                shortDisplayName = "Acceptance Resource",
            });
        Resource actualResource = await GetResourceAsync(createdResource.Id);

        // Then
        actualResource.Should().NotBeNull();
        actualResource!.Name.Should().Be(expectedResource.Name);

        await DeleteResourceAsync(createdResource.Id);
    }
}






