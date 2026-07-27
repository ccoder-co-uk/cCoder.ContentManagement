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
    private string ResourceBaseUrl { get; } = "/Api/ContentManagement/Resource";
    private static JsonSerializerOptions JsonOptions { get; } = new() { PropertyNameCaseInsensitive = true };

    private static string Unique(string prefix) =>
        $"{prefix}-{Guid.NewGuid():N}";

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

        scopedConnectionString.Should()
            .Contain(expected: "accept", because: scopedConnectionString);

        App app = await core.AddAppAsync(app: new App
        {
            Name = Unique(prefix: "AcceptanceApp"),
            Domain = $"{Unique(prefix: "acceptance")}.local",
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

        return new SeededApp(AppId: app.Id, RoleId: role.Id, Domain: app.Domain);
    }

    private async Task Teardown(SeededApp seededApp)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();

        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        UserRole[] userRoles = core
            .Set<UserRole>()
            .IgnoreQueryFilters()
            .Where(predicate: userRole => userRole.RoleId == seededApp.RoleId)
            .ToArray();

        if (userRoles.Length > 0)
        {
            await core.DeleteAllAsync(userRoles: userRoles);
        }

        Role role = core.Set<Role>()
            .IgnoreQueryFilters()
            .FirstOrDefault(predicate: foundRole => foundRole.Id == seededApp.RoleId);

        if (role is not null)
        {
            await core.DeleteAsync(role: role);
        }

        App app = core.Set<App>()
            .IgnoreQueryFilters()
            .FirstOrDefault(predicate: foundApp => foundApp.Id == seededApp.AppId);

        if (app is not null)
        {
            await core.DeleteAsync(app: app);
        }
    }

    private async Task<bool> AppExists(int appId)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();

        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return await Task.FromResult(result: core.Set<App>()
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Any(predicate: app => app.Id == appId));
    }

    private async Task<App> CreateAppAsync(object payload)
    {
        using HttpResponseMessage response = await Client.PostAsJsonAsync(requestUri: BaseUrl, value: payload);
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return JsonSerializer.Deserialize<App>(json: content, options: JsonOptions)!;
    }

    private async Task<int> UpdateAppAsync(int id, object payload)
    {
        using HttpResponseMessage response = await Client.PutAsJsonAsync(requestUri: $"{BaseUrl}({id})", value: payload);
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return (int)response.StatusCode;
    }

    private async Task<int> UpdateAppAsync(string host, int id, object payload)
    {
        using HttpRequestMessage request = new(method: HttpMethod.Put, requestUri: $"{BaseUrl}({id})")
        {
            Content = JsonContent.Create(inputValue: payload),
        };

        request.Headers.Host = host;

        using HttpResponseMessage response = await Client.SendAsync(request: request);
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return (int)response.StatusCode;
    }

    private async Task<int> PatchAppAsync(int id, object payload)
    {
        using HttpRequestMessage request = new(method: HttpMethod.Patch, requestUri: $"{BaseUrl}({id})")
        {
            Content = JsonContent.Create(inputValue: payload),
        };

        using HttpResponseMessage response = await Client.SendAsync(request: request);
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return (int)response.StatusCode;
    }

    private async Task<int> PatchAppAsync(string host, int id, object payload)
    {
        using HttpRequestMessage request = new(method: HttpMethod.Patch, requestUri: $"{BaseUrl}({id})")
        {
            Content = JsonContent.Create(inputValue: payload),
        };

        request.Headers.Host = host;

        using HttpResponseMessage response = await Client.SendAsync(request: request);
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return (int)response.StatusCode;
    }

    private async Task<int> DeleteAppAsync(int id)
    {
        using HttpResponseMessage response = await Client.DeleteAsync(requestUri: $"{BaseUrl}({id})");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return (int)response.StatusCode;
    }

    private async Task<int> DeleteAppAsync(string host, int id)
    {
        using HttpRequestMessage request = new(method: HttpMethod.Delete, requestUri: $"{BaseUrl}({id})");
        request.Headers.Host = host;

        using HttpResponseMessage response = await Client.SendAsync(request: request);
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return (int)response.StatusCode;
    }

    private async Task<App> GetAppAsync(int id)
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}({id})");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        if (content.Contains(value: "\"value\":[]", comparisonType: StringComparison.Ordinal))
        {
            return null;
        }

        return JsonSerializer.Deserialize<App>(json: content, options: JsonOptions);
    }

    private async Task<App> GetAppAsync(string host, int id)
    {
        using HttpRequestMessage request = new(method: HttpMethod.Get, requestUri: $"{BaseUrl}({id})");
        request.Headers.Host = host;

        using HttpResponseMessage response = await Client.SendAsync(request: request);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        if (content.Contains(value: "\"value\":[]", comparisonType: StringComparison.Ordinal))
        {
            return null;
        }

        return JsonSerializer.Deserialize<App>(json: content, options: JsonOptions);
    }

    private async Task<int> GetAppCountAsync()
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}/$count");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return int.Parse(s: content);
    }

    private async Task<IReadOnlyList<Layout>> GetLayoutsAsync(string host)
    {
        using HttpRequestMessage request = new(method: HttpMethod.Get, requestUri: LayoutBaseUrl);
        request.Headers.Host = host;

        using HttpResponseMessage response = await Client.SendAsync(request: request);
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return JsonSerializer.Deserialize<ODataEnvelope<Layout>>(json: content, options: JsonOptions)!.Value;
    }

    private async Task<IReadOnlyList<Resource>> GetResourcesAsync(string host)
    {
        using HttpRequestMessage request = new(method: HttpMethod.Get, requestUri: ResourceBaseUrl);
        request.Headers.Host = host;

        using HttpResponseMessage response = await Client.SendAsync(request: request);
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return JsonSerializer.Deserialize<ODataEnvelope<Resource>>(json: content, options: JsonOptions)!.Value;
    }

    private async Task<int> GetAppStatusCodeAsync(int id)
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}({id})");
        return (int)response.StatusCode;
    }

    private async Task<int> GetAppStatusCodeAsync(string host, int id)
    {
        using HttpRequestMessage request = new(method: HttpMethod.Get, requestUri: $"{BaseUrl}({id})");
        request.Headers.Host = host;

        using HttpResponseMessage response = await Client.SendAsync(request: request);
        return (int)response.StatusCode;
    }

    private async Task<AppChildCounts> GetAppChildCountsAsync(int appId)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();

        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        int roleCount = core.Set<Role>()
            .IgnoreQueryFilters()
            .Count(predicate: role => role.AppId == appId);

        Guid[] roleIds = [.. core.Set<Role>()
            .IgnoreQueryFilters()
            .Where(predicate: role => role.AppId == appId)
            .Select(selector: role => role.Id)];

        return await Task.FromResult(
result: new AppChildCounts(
AppExists: core.Set<App>()
            .IgnoreQueryFilters()
            .Any(predicate: app => app.Id == appId),
AppCultureCount: core.Set<AppCulture>()
            .IgnoreQueryFilters()
            .Count(predicate: appCulture => appCulture.AppId == appId),
ComponentCount: core.Set<Component>()
            .IgnoreQueryFilters()
            .Count(predicate: component => component.AppId == appId),
LayoutCount: core.Set<Layout>()
            .IgnoreQueryFilters()
            .Count(predicate: layout => layout.AppId == appId),
ResourceCount: core.Set<Resource>()
            .IgnoreQueryFilters()
            .Count(predicate: resource => resource.AppId == appId),
RoleCount: roleCount,
ScriptCount: core.Set<Script>()
            .IgnoreQueryFilters()
            .Count(predicate: script => script.AppId == appId),
TemplateCount: core.Set<Template>()
            .IgnoreQueryFilters()
            .Count(predicate: template => template.AppId == appId),
UserRoleCount: core.Set<UserRole>()
            .IgnoreQueryFilters()
            .Count(predicate: userRole => roleIds.Contains(value: userRole.RoleId))
            ));
    }

    private async Task<AppCmsChildren> GetAppCmsChildrenAsync(int appId)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();

        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return await Task.FromResult(
result: new AppCmsChildren(
Cultures: [.. core.Set<AppCulture>()
            .IgnoreQueryFilters()
            .Where(predicate: appCulture => appCulture.AppId == appId)],
Components: [.. core.Set<Component>()
            .IgnoreQueryFilters()
            .Where(predicate: component => component.AppId == appId)],
Layouts: [.. core.Set<Layout>()
            .IgnoreQueryFilters()
            .Where(predicate: layout => layout.AppId == appId)],
Resources: [.. core.Set<Resource>()
            .IgnoreQueryFilters()
            .Where(predicate: resource => resource.AppId == appId)]
                ,
Roles: [.. core.Set<Role>()
            .IgnoreQueryFilters()
            .Where(predicate: role => role.AppId == appId)],
Scripts: [.. core.Set<Script>()
            .IgnoreQueryFilters()
            .Where(predicate: script => script.AppId == appId)],
Templates: [.. core.Set<Template>()
            .IgnoreQueryFilters()
            .Where(predicate: template => template.AppId == appId)]
            ));
    }

    private async Task<string> GetNonDefaultCultureIdAsync()
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();

        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return await Task.FromResult(
result: core.Set<Culture>()
            .IgnoreQueryFilters()
            .Where(predicate: culture => culture.Id != string.Empty)
            .Select(selector: culture => culture.Id)
            .First());
    }

    private async Task<IReadOnlyList<string>> GetNonDefaultCultureIdsAsync(int count)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();

        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return await Task.FromResult<IReadOnlyList<string>>(
result: [.. core.Set<Culture>()
            .IgnoreQueryFilters()
            .Where(predicate: culture => culture.Id != string.Empty)
            .Select(selector: culture => culture.Id)
            .Take(count: count)]);
    }
}