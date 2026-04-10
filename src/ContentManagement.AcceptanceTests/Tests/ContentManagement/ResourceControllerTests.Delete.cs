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

        // When
        int actualStatusCode = await DeleteResourceAsync(createdResource.Id);
        int actualReadStatusCode = await GetResourceStatusCodeAsync(createdResource.Id);

        // Then
        actualStatusCode.Should().Be(200);
        actualReadStatusCode.Should().Be(404);
    }
}





