// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
    private string BaseUrl { get; } = "/Api/ContentManagement/AppCulture";
    private static JsonSerializerOptions JsonOptions { get; } = new() { PropertyNameCaseInsensitive = true };

    private static string Unique(string prefix) =>
        $"{prefix}-{Guid.NewGuid():N}";

    private sealed record SeededAppCultureContext(int AppId, Guid RoleId, string CultureId);
    private sealed record ODataEnvelope<T>(List<T> Value);

    private async Task<SeededAppCultureContext> SeedDatabase(bool includeAppCulture = false, params string[] privileges)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();

        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        App app = await core.AddAppAsync(app: new App
        {
            Name = Unique(prefix: "AcceptanceApp"),
            Domain = $"{Unique(prefix: "appculture")}.local",
            DefaultTheme = "Default",
            DefaultCultureId = string.Empty,
            TenantId = Unique(prefix: "tenant"),
            ConfigJson = "{}",
        });

        Role role = await core.AddRoleAsync(role: new Role
        {
            Id = Guid.NewGuid(),
            AppId = app.Id,
            Name = Unique(prefix: "AcceptanceRole"),
            Description = "Acceptance role",
            Privs = string.Join(separator: ',', value: privileges),
        });

        await core.AddUserRoleAsync(userRole: new UserRole { RoleId = role.Id, UserId = "Guest" });

        string cultureId = Unique(prefix: "culture");
        await core.AddCultureAsync(culture: new Culture { Id = cultureId, Name = Unique(prefix: "Culture") });

        if (includeAppCulture)
        {
            await core.AddAppCultureAsync(appCulture: new AppCulture { AppId = app.Id, CultureId = cultureId });
        }

        return new SeededAppCultureContext(AppId: app.Id, RoleId: role.Id, CultureId: cultureId);
    }

    private async Task<string> CreateCultureAsync()
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();

        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        string cultureId = Unique(prefix: "culture");

        await core.AddCultureAsync(culture: new Culture
        {
            Id = cultureId,
            Name = Unique(prefix: "Culture"),
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
            .Set<AppCulture>()
            .IgnoreQueryFilters()
            .Where(predicate: appCulture => appCulture.AppId == seededContext.AppId)
            .ToArray();

        if (appCultures.Length > 0)
        {
            await core.DeleteAllAsync(appCultures: appCultures);
        }

        UserRole[] userRoles = core
            .Set<UserRole>()
            .IgnoreQueryFilters()
            .Where(predicate: userRole => userRole.RoleId == seededContext.RoleId)
            .ToArray();

        if (userRoles.Length > 0)
        {
            await core.DeleteAllAsync(userRoles: userRoles);
        }

        Role role = core.Set<Role>()
            .IgnoreQueryFilters()
            .FirstOrDefault(predicate: foundRole => foundRole.Id == seededContext.RoleId);

        if (role is not null)
        {
            await core.DeleteAsync(role: role);
        }

        App app = core.Set<App>()
            .IgnoreQueryFilters()
            .FirstOrDefault(predicate: foundApp => foundApp.Id == seededContext.AppId);

        if (app is not null)
        {
            await core.DeleteAsync(app: app);
        }

        string[] cultureIds = [seededContext.CultureId, .. extraCultureIds];

        Culture[] cultures = core
            .Set<Culture>()
            .IgnoreQueryFilters()
            .Where(predicate: culture => cultureIds.Contains(value: culture.Id))
            .ToArray();

        if (cultures.Length > 0)
        {
            await core.DeleteAllAsync(cultures: cultures);
        }
    }

    private async Task<AppCulture> CreateAppCultureAsync(object payload)
    {
        using HttpResponseMessage response = await Client.PostAsJsonAsync(requestUri: BaseUrl, value: payload);
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.Created, because: content);

        return JsonSerializer.Deserialize<AppCulture>(json: content, options: JsonOptions)!;
    }

    private async Task<int> GetAppCultureCountAsync()
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}/$count");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return int.Parse(s: content);
    }

    private async Task<IReadOnlyList<AppCulture>> GetAppCulturesAsync(int top)
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}?$top={top}");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return JsonSerializer.Deserialize<ODataEnvelope<AppCulture>>(json: content, options: JsonOptions)!.Value;
    }

    private async Task<AppCulture> FindAppCultureAsync(int appId, string cultureId)
    {
        IReadOnlyList<AppCulture> appCultures = await GetAppCulturesAsync(top: 200);

        return appCultures.FirstOrDefault(predicate: appCulture =>
            appCulture.AppId == appId && appCulture.CultureId == cultureId
        );
    }

}