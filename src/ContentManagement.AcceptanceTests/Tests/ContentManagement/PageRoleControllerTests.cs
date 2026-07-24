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
public sealed partial class PageRoleControllerTests(WebAcceptanceFixture fixture)
{
    private HttpClient Client { get; } = fixture.Client;
    private string BaseUrl { get; } = "/Api/Core/PageRole";
    private static JsonSerializerOptions JsonOptions { get; } = new() { PropertyNameCaseInsensitive = true };

    private static string Unique(string prefix) =>
        $"{prefix}-{Guid.NewGuid():N}";

    private sealed record SeededPageRoleContext(int AppId, Guid AccessRoleId, Guid RoleId, int PageId);
    private sealed record ODataEnvelope<T>(List<T> Value);

    private async Task<SeededPageRoleContext> SeedDatabase(bool includePageRole = false, params string[] privileges)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();

        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        App app = await core.AddAppAsync(app: new App
        {
            Name = Unique(prefix: "AcceptanceApp"),
            Domain = $"{Unique(prefix: "pagerole")}.local",
            DefaultTheme = "Default",
            DefaultCultureId = string.Empty,
            TenantId = Unique(prefix: "tenant"),
            ConfigJson = "{}",
        });

        Role accessRole = await core.AddRoleAsync(role: new Role
        {
            Id = Guid.NewGuid(),
            AppId = app.Id,
            Name = Unique(prefix: "AccessRole"),
            Description = "Acceptance access role",
            Privs = string.Join(separator: ',', value: privileges),
        });

        await core.AddUserRoleAsync(userRole: new UserRole { RoleId = accessRole.Id, UserId = "Guest" });

        Role role = await core.AddRoleAsync(role: new Role
        {
            Id = Guid.NewGuid(),
            AppId = app.Id,
            Name = Unique(prefix: "TargetRole"),
            Description = "Acceptance target role",
            Privs = "pagerole_read",
        });

        Page page = await core.AddPageAsync(page: new Page
        {
            AppId = app.Id,
            Name = Unique(prefix: "Page"),
            Path = Unique(prefix: "page"),
            Layout = string.Empty,
            ShowOnMenus = true,
            Order = 1,
        });

        await core.AddPageRoleAsync(pageRole: new PageRole
        {
            PageId = page.Id,
            RoleId = accessRole.Id,
        });

        if (includePageRole)
        {
            await core.AddPageRoleAsync(pageRole: new PageRole
            {
                PageId = page.Id,
                RoleId = role.Id,
            });
        }

        return new SeededPageRoleContext(AppId: app.Id, AccessRoleId: accessRole.Id, RoleId: role.Id, PageId: page.Id);
    }

    private async Task Teardown(SeededPageRoleContext seededContext)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();

        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        PageRole[] pageRoles = core
            .Set<PageRole>()
            .IgnoreQueryFilters()
            .Where(predicate: pageRole => pageRole.PageId == seededContext.PageId)
            .ToArray();

        if (pageRoles.Length > 0)
        {
            await core.DeleteAllAsync(pageRoles: pageRoles);
        }

        UserRole[] userRoles = core
            .Set<UserRole>()
            .IgnoreQueryFilters()
            .Where(predicate: userRole => userRole.RoleId == seededContext.AccessRoleId)
            .ToArray();

        if (userRoles.Length > 0)
        {
            await core.DeleteAllAsync(userRoles: userRoles);
        }

        Page page = core.Set<Page>()
            .IgnoreQueryFilters()
            .FirstOrDefault(predicate: foundPage => foundPage.Id == seededContext.PageId);

        if (page is not null)
        {
            await core.DeleteAsync(page: page);
        }

        Role[] roles = core.Set<Role>()
            .IgnoreQueryFilters()
            .Where(predicate: foundRole => foundRole.Id == seededContext.AccessRoleId || foundRole.Id == seededContext.RoleId)
            .ToArray();

        if (roles.Length > 0)
        {
            await core.DeleteAllAsync(roles: roles);
        }

        App app = core.Set<App>()
            .IgnoreQueryFilters()
            .FirstOrDefault(predicate: foundApp => foundApp.Id == seededContext.AppId);

        if (app is not null)
        {
            await core.DeleteAsync(app: app);
        }
    }

    private async Task<PageRole> CreatePageRoleAsync(object payload)
    {
        using HttpResponseMessage response = await Client.PostAsJsonAsync(requestUri: BaseUrl, value: payload);
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return JsonSerializer.Deserialize<PageRole>(json: content, options: JsonOptions)!;
    }

    private async Task<int> GetPageRoleCountAsync()
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}/$count");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return int.Parse(s: content);
    }

    private async Task<IReadOnlyList<PageRole>> GetPageRolesAsync(int top)
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}?$top={top}");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return JsonSerializer.Deserialize<ODataEnvelope<PageRole>>(json: content, options: JsonOptions)!.Value;
    }

    private async Task<PageRole> FindPageRoleAsync(int pageId, Guid roleId)
    {
        IReadOnlyList<PageRole> pageRoles = await GetPageRolesAsync(top: 200);

        return pageRoles.FirstOrDefault(predicate: pageRole =>
            pageRole.PageId == pageId && pageRole.RoleId == roleId
        );
    }

}