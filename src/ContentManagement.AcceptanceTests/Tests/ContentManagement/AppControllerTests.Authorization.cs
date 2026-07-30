// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Exposures;
using cCoder.Data;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Microsoft.EntityFrameworkCore;
namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class AppControllerTests
{
    [Fact]
    public async Task Post_CreatesBootstrapAdministratorRoleForCreatedApp()
    {
        // Given
        SeededApp seededApp = await SeedDatabase(privileges: "app_create");

        App createdApp = await CreateAppAsync(
payload: new
{
    name = Unique(prefix: "AuthApp"),
    domain = $"{Unique(prefix: "auth")}.local",
    defaultTheme = "Default",
    defaultCultureId = string.Empty,
    tenantId = Unique(prefix: "tenant"),
    configJson = "{}",
});

        using IServiceScope scope = fixture.Factory.Services.CreateScope();

        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        IAuthorizationManager authorizationManager =
            scope.ServiceProvider.GetRequiredService<IAuthorizationManager>();

        Role[] roles = [.. core.Set<Role>()
            .IgnoreQueryFilters()
            .Where(predicate: role => role.AppId == createdApp.Id)];

        // When
        UserRole[] userRoles = [.. core.Set<UserRole>()
            .IgnoreQueryFilters()
            .Where(predicate: userRole => roles.Select(selector: role => role.Id)
            .Contains(value: userRole.RoleId))];

        // Then
        roles.Should()
            .Contain(predicate: role =>
            string.Equals(a: role.Name, b: "Administrators", comparisonType: StringComparison.OrdinalIgnoreCase)
            && role.Privileges.Contains(value: "app_admin", comparer: StringComparer.OrdinalIgnoreCase));

        userRoles.Should()
            .Contain(predicate: userRole => userRole.UserId == "Guest");

        authorizationManager.IsAdminOfApp(appId: createdApp.Id)
            .Should()
            .BeTrue();

        await DeleteAppAsync(host: createdApp.Domain, id: createdApp.Id);
        await Teardown(seededApp: seededApp);
    }
}