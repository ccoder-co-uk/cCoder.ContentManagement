using cCoder.ContentManagement.Brokers;
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
    public async Task Post_CreatesGuestAdministratorRoleForCreatedApp()
    {
        SeededApp seededApp = await SeedDatabase("app_create");
        App createdApp = await CreateAppAsync(
            new
            {
                name = Unique("AuthApp"),
                domain = $"{Unique("auth")}.local",
                defaultTheme = "Default",
                defaultCultureId = string.Empty,
                tenantId = Unique("tenant"),
                configJson = "{}",
            });

        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();
        IAuthorizationBroker authorizationBroker = scope.ServiceProvider.GetRequiredService<IAuthorizationBroker>();

        Role[] roles = [.. core.Set<Role>().IgnoreQueryFilters().Where(role => role.AppId == createdApp.Id)];
        UserRole[] userRoles = [.. core.Set<UserRole>().IgnoreQueryFilters()
            .Where(userRole => roles.Select(role => role.Id).Contains(userRole.RoleId))];

        roles.Should().Contain(role =>
            string.Equals(role.Name, "Administrators", StringComparison.OrdinalIgnoreCase)
            && role.Privileges.Contains("app_admin", StringComparer.OrdinalIgnoreCase));
        userRoles.Should().Contain(userRole => userRole.UserId == "Guest");
        authorizationBroker.IsAdminOfApp(createdApp.Id).Should().BeTrue();

        await DeleteAppAsync(createdApp.Domain, createdApp.Id);
        await Teardown(seededApp);
    }
}



