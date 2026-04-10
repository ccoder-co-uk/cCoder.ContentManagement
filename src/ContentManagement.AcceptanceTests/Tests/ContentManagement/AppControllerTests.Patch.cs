using cCoder.Data.Models.CMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class AppControllerTests
{
    [Fact]
    public async Task Patch_UpdatesApp()
    {
        // Given
        SeededApp seededApp = await SeedDatabase("app_update", "app_read", "app_delete");
        App expectedApp = new() { Id = seededApp.AppId, DefaultTheme = "Patched" };

        // When
        await PatchAppAsync(seededApp.Domain, seededApp.AppId, new { defaultTheme = "Patched" });
        App actualApp = await GetAppAsync(seededApp.Domain, seededApp.AppId);

        // Then
        actualApp.Should().NotBeNull();
        actualApp!.Id.Should().Be(expectedApp.Id);
        actualApp.DefaultTheme.Should().Be(expectedApp.DefaultTheme);

        await DeleteAppAsync(seededApp.Domain, seededApp.AppId);

        await Teardown(seededApp);
    }
}






