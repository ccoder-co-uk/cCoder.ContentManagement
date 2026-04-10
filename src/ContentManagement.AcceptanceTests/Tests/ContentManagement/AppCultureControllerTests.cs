using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using cCoder.Data;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Web.AcceptanceTests.Infrastructure;
using Xunit;


using Microsoft.EntityFrameworkCore;
namespace Web.AcceptanceTests.Tests.ContentManagement;

[Collection(WebAcceptanceCollection.Name)]
public sealed partial class AppCultureControllerTests(WebAcceptanceFixture fixture)
{
    private HttpClient Client { get; } = fixture.Client;
    private string BaseUrl { get; } = "/Api/Core/AppCulture";
    private static JsonSerializerOptions JsonOptions { get; } = new() { PropertyNameCaseInsensitive = true };

    private static string Unique(string prefix) => $"{prefix}-{Guid.NewGuid():N}";

    private sealed record SeededAppCultureContext(int AppId, Guid RoleId, string CultureId);
    private sealed record ODataEnvelope<T>(List<T> Value);

    private async Task<SeededAppCultureContext> SeedDatabase(bool includeAppCulture = false, params string[] privileges)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        App app = await core.AddAppAsync(new App
        {
            Name = Unique("AcceptanceApp"),
            Domain = $"{Unique("appculture")}.local",
            DefaultTheme = "Default",
            DefaultCultureId = string.Empty,
            TenantId = Unique("tenant"),
            ConfigJson = "{}",
        });

        Role role = await core.AddRoleAsync(new Role
        {
            Id = Guid.NewGuid(),
            AppId = app.Id,
            Name = Unique("AcceptanceRole"),
            Description = "Acceptance role",
            Privs = string.Join(',', privileges),
        });

        await core.AddUserRoleAsync(new UserRole { RoleId = role.Id, UserId = "Guest" });

        string cultureId = Unique("culture");
        await core.AddCultureAsync(new Culture { Id = cultureId, Name = Unique("Culture") });

        if (includeAppCulture)
            await core.AddAppCultureAsync(new AppCulture { AppId = app.Id, CultureId = cultureId });

        return new SeededAppCultureContext(app.Id, role.Id, cultureId);
    }

    private async Task<string> CreateCultureAsync()
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();
        string cultureId = Unique("culture");

        await core.AddCultureAsync(new Culture
        {
            Id = cultureId,
            Name = Unique("Culture"),
        });

        return cultureId;
    }

    private async Task Teardown(SeededAppCultureContext seededContext, params string[] extraCultureIds)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        AppCulture[] appCultures = core
            .Set<AppCulture>().IgnoreQueryFilters()
            .Where(appCulture => appCulture.AppId == seededContext.AppId)
            .ToArray();

        if (appCultures.Length > 0)
            await core.DeleteAllAsync(appCultures);

        UserRole[] userRoles = core
            .Set<UserRole>().IgnoreQueryFilters()
            .Where(userRole => userRole.RoleId == seededContext.RoleId)
            .ToArray();

        if (userRoles.Length > 0)
            await core.DeleteAllAsync(userRoles);

        Role role = core.Set<Role>().IgnoreQueryFilters().FirstOrDefault(foundRole => foundRole.Id == seededContext.RoleId);
        if (role is not null)
            await core.DeleteAsync(role);

        App app = core.Set<App>().IgnoreQueryFilters().FirstOrDefault(foundApp => foundApp.Id == seededContext.AppId);
        if (app is not null)
            await core.DeleteAsync(app);

        string[] cultureIds = [seededContext.CultureId, .. extraCultureIds];
        Culture[] cultures = core
            .Set<Culture>().IgnoreQueryFilters()
            .Where(culture => cultureIds.Contains(culture.Id))
            .ToArray();

        if (cultures.Length > 0)
            await core.DeleteAllAsync(cultures);
    }

    private async Task<AppCulture> CreateAppCultureAsync(object payload)
    {
        using HttpResponseMessage response = await Client.PostAsJsonAsync(BaseUrl, payload);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return JsonSerializer.Deserialize<AppCulture>(content, JsonOptions)!;
    }

    private async Task<int> GetAppCultureCountAsync()
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}/$count");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return int.Parse(content);
    }

    private async Task<IReadOnlyList<AppCulture>> GetAppCulturesAsync(int top)
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}?$top={top}");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return JsonSerializer.Deserialize<ODataEnvelope<AppCulture>>(content, JsonOptions)!.Value;
    }

    private async Task<AppCulture> FindAppCultureAsync(int appId, string cultureId)
    {
        IReadOnlyList<AppCulture> appCultures = await GetAppCulturesAsync(200);
        return appCultures.FirstOrDefault(appCulture =>
            appCulture.AppId == appId && appCulture.CultureId == cultureId
        );
    }

}








