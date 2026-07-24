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
    public async Task Post_CreatesApp()
    {
        // Given
        SeededApp seededApp = await SeedDatabase(privileges: ["app_create", "app_read", "app_delete"]);
        string createdName = Unique(prefix: "CreatedApp");
        App expectedApp = new() { Name = createdName };

        // When
        App createdApp = await CreateAppAsync(
payload: new
{
    name = createdName,
    domain = $"{Unique(prefix: "created")}.local",
    defaultTheme = "Default",
    defaultCultureId = string.Empty,
    tenantId = Unique(prefix: "tenant"),
    configJson = "{}",
});

        App actualApp = await GetAppAsync(host: createdApp.Domain, id: createdApp.Id);

        // Then
        actualApp.Should()
            .NotBeNull();

        actualApp!.Name.Should()
            .Be(expected: expectedApp.Name);

        await DeleteAppAsync(host: createdApp.Domain, id: createdApp.Id);

        await Teardown(seededApp: seededApp);
    }
}