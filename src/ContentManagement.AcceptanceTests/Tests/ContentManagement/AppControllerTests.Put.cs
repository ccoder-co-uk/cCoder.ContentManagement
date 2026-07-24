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
    public async Task Put_UpdatesApp()
    {
        // Given
        SeededApp seededApp = await SeedDatabase("app_update", "app_read", "app_delete");
        string updatedName = Unique(prefix: "UpdatedApp");
        string updatedDomain = $"{Unique(prefix: "updated")}.local";
        App expectedApp = new() { Id = seededApp.AppId, Name = updatedName, DefaultTheme = "Updated", Domain = updatedDomain };

        // When

        await UpdateAppAsync(
host: seededApp.Domain,
id: seededApp.AppId,
payload: new
{
    id = seededApp.AppId,
    name = updatedName,
    domain = updatedDomain,
    defaultTheme = "Updated",
    defaultCultureId = string.Empty,
    tenantId = Unique(prefix: "tenant"),
    configJson = "{\"updated\":true}",
});

        App actualApp = await GetAppAsync(host: updatedDomain, id: seededApp.AppId);

        // Then

        actualApp.Should()
            .NotBeNull();

        actualApp!.Id.Should()
            .Be(expected: expectedApp.Id);

        actualApp.Name.Should()
            .Be(expected: expectedApp.Name);

        actualApp.DefaultTheme.Should()
            .Be(expected: expectedApp.DefaultTheme);

        actualApp.Domain.Should()
            .Be(expected: expectedApp.Domain);

        await DeleteAppAsync(host: updatedDomain, id: seededApp.AppId);

        await Teardown(seededApp: seededApp);
    }
}