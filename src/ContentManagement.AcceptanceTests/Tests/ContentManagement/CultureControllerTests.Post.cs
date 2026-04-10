using cCoder.Data.Models.CMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class CultureControllerTests
{
    [Fact]
    public async Task Post_CreatesCulture()
    {
        // Given
        string cultureId = Unique("culture");
        Culture expectedCulture = new() { Id = cultureId };

        // When
        await CreateCultureAsync(new
        {
            id = cultureId,
            name = Unique("Culture"),
        });

        Culture actualCulture = await GetCultureAsync(cultureId);

        // Then
        actualCulture.Should().NotBeNull();
        actualCulture!.Id.Should().Be(expectedCulture.Id);

        await DeleteCultureAsync(cultureId);
        await Teardown(cultureId);
    }
}






