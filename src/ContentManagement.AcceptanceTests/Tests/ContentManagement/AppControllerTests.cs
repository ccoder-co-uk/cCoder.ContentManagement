using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using cCoder.Data;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Web.AcceptanceTests.Infrastructure;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

[Collection(WebAcceptanceCollection.Name)]
public sealed partial class AppControllerTests(WebAcceptanceFixture fixture)
{
    private HttpClient Client { get; } = fixture.Client;
    private string BaseUrl { get; } = "/Api/Core/App";
    private string LayoutBaseUrl { get; } = "/Api/Core/Layout";
    private string ResourceBaseUrl { get; } = "/Api/Core/Resource";
    private static JsonSerializerOptions JsonOptions { get; } = new() { PropertyNameCaseInsensitive = true };

    private static string Unique(string prefix) => $"{prefix}-{Guid.NewGuid():N}";

    private sealed record SeededApp(int AppId, Guid RoleId, string Domain);
    private sealed record ODataEnvelope<T>(List<T> Value);
    private sealed record AppChildCounts(
        bool AppExists,
        int AppCultureCount,
        int ComponentCount,
        int LayoutCount,
        int ResourceCount,
        int RoleCount,
        int ScriptCount,
        int TemplateCount,
        int UserRoleCount
    );
    private sealed record AppCmsChildren(
        IReadOnlyList<AppCulture> Cultures,
        IReadOnlyList<Component> Components,
        IReadOnlyList<Layout> Layouts,
        IReadOnlyList<Resource> Resources,
        IReadOnlyList<Role> Roles,
        IReadOnlyList<Script> Scripts,
        IReadOnlyList<Template> Templates
    );

    private async Task<SeededApp> SeedDatabase(params string[] privileges)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();
        string scopedConnectionString = core.Database.GetConnectionString() ?? string.Empty;

        scopedConnectionString.Should().Contain("accept", because: scopedConnectionString);

        App app = await core.AddAppAsync(new App
            {
                Name = Unique("AcceptanceApp"),
                Domain = $"{Unique("acceptance")}.local",
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

        return new SeededApp(app.Id, role.Id, app.Domain);
    }

    private async Task Teardown(SeededApp seededApp)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        UserRole[] userRoles = core
            .Set<UserRole>().IgnoreQueryFilters()
            .Where(userRole => userRole.RoleId == seededApp.RoleId)
            .ToArray();

        if (userRoles.Length > 0)
            await core.DeleteAllAsync(userRoles);

        Role role = core.Set<Role>().IgnoreQueryFilters().FirstOrDefault(foundRole => foundRole.Id == seededApp.RoleId);
        if (role is not null)
            await core.DeleteAsync(role);

        App app = core.Set<App>().IgnoreQueryFilters().FirstOrDefault(foundApp => foundApp.Id == seededApp.AppId);
        if (app is not null)
            await core.DeleteAsync(app);
    }

    private async Task<bool> AppExists(int appId)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();
        return await Task.FromResult(core.Set<App>().IgnoreQueryFilters().AsNoTracking().Any(app => app.Id == appId));
    }

    private async Task<App> CreateAppAsync(object payload)
    {
        using HttpResponseMessage response = await Client.PostAsJsonAsync(BaseUrl, payload);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return JsonSerializer.Deserialize<App>(content, JsonOptions)!;
    }

    private async Task<int> UpdateAppAsync(int id, object payload)
    {
        using HttpResponseMessage response = await Client.PutAsJsonAsync($"{BaseUrl}({id})", payload);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return (int)response.StatusCode;
    }

    private async Task<int> UpdateAppAsync(string host, int id, object payload)
    {
        using HttpRequestMessage request = new(HttpMethod.Put, $"{BaseUrl}({id})")
        {
            Content = JsonContent.Create(payload),
        };
        request.Headers.Host = host;

        using HttpResponseMessage response = await Client.SendAsync(request);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return (int)response.StatusCode;
    }

    private async Task<int> PatchAppAsync(int id, object payload)
    {
        using HttpRequestMessage request = new(HttpMethod.Patch, $"{BaseUrl}({id})")
        {
            Content = JsonContent.Create(payload),
        };

        using HttpResponseMessage response = await Client.SendAsync(request);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return (int)response.StatusCode;
    }

    private async Task<int> PatchAppAsync(string host, int id, object payload)
    {
        using HttpRequestMessage request = new(HttpMethod.Patch, $"{BaseUrl}({id})")
        {
            Content = JsonContent.Create(payload),
        };
        request.Headers.Host = host;

        using HttpResponseMessage response = await Client.SendAsync(request);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return (int)response.StatusCode;
    }

    private async Task<int> DeleteAppAsync(int id)
    {
        using HttpResponseMessage response = await Client.DeleteAsync($"{BaseUrl}({id})");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return (int)response.StatusCode;
    }

    private async Task<int> DeleteAppAsync(string host, int id)
    {
        using HttpRequestMessage request = new(HttpMethod.Delete, $"{BaseUrl}({id})");
        request.Headers.Host = host;

        using HttpResponseMessage response = await Client.SendAsync(request);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return (int)response.StatusCode;
    }

    private async Task<App> GetAppAsync(int id)
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}({id})");

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);

        if (content.Contains("\"value\":[]", StringComparison.Ordinal))
            return null;

        return JsonSerializer.Deserialize<App>(content, JsonOptions);
    }

    private async Task<App> GetAppAsync(string host, int id)
    {
        using HttpRequestMessage request = new(HttpMethod.Get, $"{BaseUrl}({id})");
        request.Headers.Host = host;

        using HttpResponseMessage response = await Client.SendAsync(request);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);

        if (content.Contains("\"value\":[]", StringComparison.Ordinal))
            return null;

        return JsonSerializer.Deserialize<App>(content, JsonOptions);
    }

    private async Task<int> GetAppCountAsync()
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}/$count");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return int.Parse(content);
    }

    private async Task<IReadOnlyList<Layout>> GetLayoutsAsync(string host)
    {
        using HttpRequestMessage request = new(HttpMethod.Get, LayoutBaseUrl);
        request.Headers.Host = host;

        using HttpResponseMessage response = await Client.SendAsync(request);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return JsonSerializer.Deserialize<ODataEnvelope<Layout>>(content, JsonOptions)!.Value;
    }

    private async Task<IReadOnlyList<Resource>> GetResourcesAsync(string host)
    {
        using HttpRequestMessage request = new(HttpMethod.Get, ResourceBaseUrl);
        request.Headers.Host = host;

        using HttpResponseMessage response = await Client.SendAsync(request);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return JsonSerializer.Deserialize<ODataEnvelope<Resource>>(content, JsonOptions)!.Value;
    }

    private async Task<int> GetAppStatusCodeAsync(int id)
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}({id})");
        return (int)response.StatusCode;
    }

    private async Task<int> GetAppStatusCodeAsync(string host, int id)
    {
        using HttpRequestMessage request = new(HttpMethod.Get, $"{BaseUrl}({id})");
        request.Headers.Host = host;

        using HttpResponseMessage response = await Client.SendAsync(request);
        return (int)response.StatusCode;
    }

    private async Task<AppChildCounts> GetAppChildCountsAsync(int appId)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        int roleCount = core.Set<Role>().IgnoreQueryFilters().Count(role => role.AppId == appId);
        Guid[] roleIds = [.. core.Set<Role>().IgnoreQueryFilters().Where(role => role.AppId == appId).Select(role => role.Id)];

        return await Task.FromResult(
            new AppChildCounts(
                core.Set<App>().IgnoreQueryFilters().Any(app => app.Id == appId),
                core.Set<AppCulture>().IgnoreQueryFilters().Count(appCulture => appCulture.AppId == appId),
                core.Set<Component>().IgnoreQueryFilters().Count(component => component.AppId == appId),
                core.Set<Layout>().IgnoreQueryFilters().Count(layout => layout.AppId == appId),
                core.Set<Resource>().IgnoreQueryFilters().Count(resource => resource.AppId == appId),
                roleCount,
                core.Set<Script>().IgnoreQueryFilters().Count(script => script.AppId == appId),
                core.Set<Template>().IgnoreQueryFilters().Count(template => template.AppId == appId),
                core.Set<UserRole>().IgnoreQueryFilters().Count(userRole => roleIds.Contains(userRole.RoleId))
            ));
    }

    private async Task<AppCmsChildren> GetAppCmsChildrenAsync(int appId)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return await Task.FromResult(
            new AppCmsChildren(
                [.. core.Set<AppCulture>().IgnoreQueryFilters().Where(appCulture => appCulture.AppId == appId)],
                [.. core.Set<Component>().IgnoreQueryFilters().Where(component => component.AppId == appId)],
                [.. core.Set<Layout>().IgnoreQueryFilters().Where(layout => layout.AppId == appId)],
                [.. core.Set<Resource>().IgnoreQueryFilters().Where(resource => resource.AppId == appId)]
                ,
                [.. core.Set<Role>().IgnoreQueryFilters().Where(role => role.AppId == appId)],
                [.. core.Set<Script>().IgnoreQueryFilters().Where(script => script.AppId == appId)],
                [.. core.Set<Template>().IgnoreQueryFilters().Where(template => template.AppId == appId)]
            ));
    }

    private async Task<string> GetNonDefaultCultureIdAsync()
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return await Task.FromResult(
            core.Set<Culture>().IgnoreQueryFilters()
                .Where(culture => culture.Id != string.Empty)
                .Select(culture => culture.Id)
                .First());
    }

    private async Task<IReadOnlyList<string>> GetNonDefaultCultureIdsAsync(int count)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return await Task.FromResult<IReadOnlyList<string>>(
            [.. core.Set<Culture>().IgnoreQueryFilters()
                .Where(culture => culture.Id != string.Empty)
                .Select(culture => culture.Id)
                .Take(count)]);
    }
}









