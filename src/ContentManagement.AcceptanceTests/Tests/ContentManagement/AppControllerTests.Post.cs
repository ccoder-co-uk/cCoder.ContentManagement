using cCoder.Data.Models.CMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class AppControllerTests
{
    [Fact]
    public async Task Post_CreatesApp()
    {
        // Given
        SeededApp seededApp = await SeedDatabase("app_create", "app_read", "app_delete");
        string createdName = Unique("CreatedApp");
        App expectedApp = new() { Name = createdName };

        // When
        App createdApp = await CreateAppAsync(
            new
            {
                name = createdName,
                domain = $"{Unique("created")}.local",
                defaultTheme = "Default",
                defaultCultureId = string.Empty,
                tenantId = Unique("tenant"),
                configJson = "{}",
            });
        App actualApp = await GetAppAsync(createdApp.Domain, createdApp.Id);

        // Then
        actualApp.Should().NotBeNull();
        actualApp!.Name.Should().Be(expectedApp.Name);

        await DeleteAppAsync(createdApp.Domain, createdApp.Id);

        await Teardown(seededApp);
    }
}






