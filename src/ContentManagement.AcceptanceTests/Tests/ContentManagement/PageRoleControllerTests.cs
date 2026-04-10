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

    private static string Unique(string prefix) => $"{prefix}-{Guid.NewGuid():N}";

    private sealed record SeededPageRoleContext(int AppId, Guid AccessRoleId, Guid RoleId, int PageId);
    private sealed record ODataEnvelope<T>(List<T> Value);

    private async Task<SeededPageRoleContext> SeedDatabase(bool includePageRole = false, params string[] privileges)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        App app = await core.AddAppAsync(new App
        {
            Name = Unique("AcceptanceApp"),
            Domain = $"{Unique("pagerole")}.local",
            DefaultTheme = "Default",
            DefaultCultureId = string.Empty,
            TenantId = Unique("tenant"),
            ConfigJson = "{}",
        });

        Role accessRole = await core.AddRoleAsync(new Role
        {
            Id = Guid.NewGuid(),
            AppId = app.Id,
            Name = Unique("AccessRole"),
            Description = "Acceptance access role",
            Privs = string.Join(',', privileges),
        });

        await core.AddUserRoleAsync(new UserRole { RoleId = accessRole.Id, UserId = "Guest" });

        Role role = await core.AddRoleAsync(new Role
        {
            Id = Guid.NewGuid(),
            AppId = app.Id,
            Name = Unique("TargetRole"),
            Description = "Acceptance target role",
            Privs = "pagerole_read",
        });

        Page page = await core.AddPageAsync(new Page
        {
            AppId = app.Id,
            Name = Unique("Page"),
            Path = Unique("page"),
            Layout = string.Empty,
            ShowOnMenus = true,
            Order = 1,
        });

        await core.AddPageRoleAsync(new PageRole
        {
            PageId = page.Id,
            RoleId = accessRole.Id,
        });

        if (includePageRole)
        {
            await core.AddPageRoleAsync(new PageRole
            {
                PageId = page.Id,
                RoleId = role.Id,
            });
        }

        return new SeededPageRoleContext(app.Id, accessRole.Id, role.Id, page.Id);
    }

    private async Task Teardown(SeededPageRoleContext seededContext)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        PageRole[] pageRoles = core
            .Set<PageRole>().IgnoreQueryFilters()
            .Where(pageRole => pageRole.PageId == seededContext.PageId)
            .ToArray();

        if (pageRoles.Length > 0)
            await core.DeleteAllAsync(pageRoles);

        UserRole[] userRoles = core
            .Set<UserRole>().IgnoreQueryFilters()
            .Where(userRole => userRole.RoleId == seededContext.AccessRoleId)
            .ToArray();

        if (userRoles.Length > 0)
            await core.DeleteAllAsync(userRoles);

        Page page = core.Set<Page>().IgnoreQueryFilters().FirstOrDefault(foundPage => foundPage.Id == seededContext.PageId);
        if (page is not null)
            await core.DeleteAsync(page);

        Role[] roles = core.Set<Role>().IgnoreQueryFilters().Where(foundRole => foundRole.Id == seededContext.AccessRoleId || foundRole.Id == seededContext.RoleId).ToArray();
        if (roles.Length > 0)
            await core.DeleteAllAsync(roles);

        App app = core.Set<App>().IgnoreQueryFilters().FirstOrDefault(foundApp => foundApp.Id == seededContext.AppId);
        if (app is not null)
            await core.DeleteAsync(app);
    }

    private async Task<PageRole> CreatePageRoleAsync(object payload)
    {
        using HttpResponseMessage response = await Client.PostAsJsonAsync(BaseUrl, payload);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return JsonSerializer.Deserialize<PageRole>(content, JsonOptions)!;
    }

    private async Task<int> GetPageRoleCountAsync()
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}/$count");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return int.Parse(content);
    }

    private async Task<IReadOnlyList<PageRole>> GetPageRolesAsync(int top)
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}?$top={top}");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return JsonSerializer.Deserialize<ODataEnvelope<PageRole>>(content, JsonOptions)!.Value;
    }

    private async Task<PageRole> FindPageRoleAsync(int pageId, Guid roleId)
    {
        IReadOnlyList<PageRole> pageRoles = await GetPageRolesAsync(200);
        return pageRoles.FirstOrDefault(pageRole =>
            pageRole.PageId == pageId && pageRole.RoleId == roleId
        );
    }

}








