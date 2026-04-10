using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class CultureControllerTests
{
    [Fact]
    public async Task Delete_RemovesCulture()
    {
        // Given
        SeededCultureContext seededContext = await SeedDatabase();

        // When
        int actualStatusCode = await DeleteCultureAsync(seededContext.CultureId);
        int actualReadStatusCode = await GetCultureStatusCodeAsync(seededContext.CultureId);

        // Then
        actualStatusCode.Should().Be(200);
        actualReadStatusCode.Should().Be(404);

        await Teardown(seededContext.CultureId);
    }
}





