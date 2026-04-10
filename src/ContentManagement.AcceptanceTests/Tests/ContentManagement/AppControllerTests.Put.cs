using cCoder.Data.Models.CMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class AppControllerTests
{
    [Fact]
    public async Task Put_UpdatesApp()
    {
        // Given
        SeededApp seededApp = await SeedDatabase("app_update", "app_read", "app_delete");
        string updatedName = Unique("UpdatedApp");
        string updatedDomain = $"{Unique("updated")}.local";
        App expectedApp = new() { Id = seededApp.AppId, Name = updatedName, DefaultTheme = "Updated", Domain = updatedDomain };

        // When
        await UpdateAppAsync(
            seededApp.Domain,
            seededApp.AppId,
            new
            {
                id = seededApp.AppId,
                name = updatedName,
                domain = updatedDomain,
                defaultTheme = "Updated",
                defaultCultureId = string.Empty,
                tenantId = Unique("tenant"),
                configJson = "{\"updated\":true}",
            });
        App actualApp = await GetAppAsync(updatedDomain, seededApp.AppId);

        // Then
        actualApp.Should().NotBeNull();
        actualApp!.Id.Should().Be(expectedApp.Id);
        actualApp.Name.Should().Be(expectedApp.Name);
        actualApp.DefaultTheme.Should().Be(expectedApp.DefaultTheme);
        actualApp.Domain.Should().Be(expectedApp.Domain);

        await DeleteAppAsync(updatedDomain, seededApp.AppId);

        await Teardown(seededApp);
    }
}






