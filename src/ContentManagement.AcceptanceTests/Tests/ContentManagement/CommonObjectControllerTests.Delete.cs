using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class CommonObjectControllerTests
{
    [Fact]
    public async Task Delete_RemovesCommonObject()
    {
        // Given
        SeededCommonObjectContext seededContext = await SeedDatabase();

        // When
        int actualStatusCode = await DeleteCommonObjectAsync(seededContext.Id);
        int actualReadStatusCode = await GetCommonObjectStatusCodeAsync(seededContext.Id);

        // Then
        actualStatusCode.Should().Be(200);
        actualReadStatusCode.Should().Be(404);

        await Teardown(seededContext.Id);
    }
}





