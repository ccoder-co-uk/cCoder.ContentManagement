using cCoder.Data.Models.CMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class CultureControllerTests
{
    [Fact]
    public async Task Patch_UpdatesCulture()
    {
        // Given
        SeededCultureContext seededContext = await SeedDatabase();
        string updatedName = Unique("PatchedCulture");
        Culture expectedCulture = new() { Name = updatedName };

        // When
        await PatchCultureAsync(seededContext.CultureId, new
        {
            name = updatedName,
        });

        Culture actualCulture = await GetCultureAsync(seededContext.CultureId);

        // Then
        actualCulture.Should().NotBeNull();
        actualCulture!.Name.Should().Be(expectedCulture.Name);

        await DeleteCultureAsync(seededContext.CultureId);
        await Teardown(seededContext.CultureId);
    }
}






