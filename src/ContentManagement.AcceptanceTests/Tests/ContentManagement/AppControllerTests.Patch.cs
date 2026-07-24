// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        await PatchAppAsync(host: seededApp.Domain, id: seededApp.AppId, payload: new { defaultTheme = "Patched" });
        App actualApp = await GetAppAsync(host: seededApp.Domain, id: seededApp.AppId);

        // Then

        actualApp.Should()
            .NotBeNull();

        actualApp!.Id.Should()
            .Be(expected: expectedApp.Id);

        actualApp.DefaultTheme.Should()
            .Be(expected: expectedApp.DefaultTheme);

        await DeleteAppAsync(host: seededApp.Domain, id: seededApp.AppId);

        await Teardown(seededApp: seededApp);
    }
}