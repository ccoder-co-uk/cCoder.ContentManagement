using cCoder.Data.Models.CMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class AppCultureControllerTests
{
    [Fact]
    public async Task Post_CreatesAppCulture()
    {
        // Given
        SeededAppCultureContext seededContext = await SeedDatabase(false, "appculture_create", "appculture_delete");
        AppCulture actualAppCulture;

        // When
        await CreateAppCultureAsync(new
        {
            appId = seededContext.AppId,
            cultureId = seededContext.CultureId,
        });

        actualAppCulture = await FindAppCultureAsync(seededContext.AppId, seededContext.CultureId);

        // Then
        actualAppCulture.Should().NotBeNull();
        actualAppCulture!.AppId.Should().Be(seededContext.AppId);
        actualAppCulture.CultureId.Should().Be(seededContext.CultureId);

        await Teardown(seededContext);
    }
}






