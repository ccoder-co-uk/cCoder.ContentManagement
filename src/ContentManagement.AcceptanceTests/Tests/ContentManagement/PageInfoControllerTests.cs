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
public sealed partial class PageInfoControllerTests(WebAcceptanceFixture fixture)
{
    private HttpClient Client { get; } = fixture.Client;
    private string BaseUrl { get; } = "/Api/Core/PageInfo";
    private static JsonSerializerOptions JsonOptions { get; } = new() { PropertyNameCaseInsensitive = true };

    private static string Unique(string prefix) => $"{prefix}-{Guid.NewGuid():N}";
    private sealed record ODataEnvelope<T>(List<T> Value);

    private sealed record SeededPageInfoContext(int AppId, Guid RoleId, int PageId, int PageInfoId);

    private async Task<SeededPageInfoContext> SeedDatabase(bool includePageInfo = false)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        App app = await core.AddAppAsync(new App
        {
            Name = Unique("AcceptanceApp"),
            Domain = $"{Unique("pageinfo")}.local",
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
            Privs = "app_admin,pageinfo_create,pageinfo_update,pageinfo_delete,pageinfo_read,page_create,page_update,page_delete,page_read",
        });

        await core.AddUserRoleAsync(new UserRole { RoleId = role.Id, UserId = "Guest" });

        Page page = await core.AddPageAsync(new Page
        {
            AppId = app.Id,
            Name = Unique("Page"),
            Path = Unique("page"),
            Layout = string.Empty,
            ShowOnMenus = true,
            Order = 1,
        });

        int pageInfoId = 0;

        if (includePageInfo)
        {
            PageInfo pageInfo = await core.AddPageInfoAsync(new PageInfo
            {
                PageId = page.Id,
                CultureId = string.Empty,
                Title = Unique("Title"),
                Description = "Acceptance page info",
                Keywords = "acceptance",
            });

            pageInfoId = pageInfo.Id;
        }

        return new SeededPageInfoContext(app.Id, role.Id, page.Id, pageInfoId);
    }

    private async Task Teardown(SeededPageInfoContext seededContext)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        PageInfo[] pageInfos = core
            .Set<PageInfo>().IgnoreQueryFilters()
            .Where(pageInfo => pageInfo.PageId == seededContext.PageId)
            .ToArray();

        if (pageInfos.Length > 0)
            await core.DeleteAllAsync(pageInfos);

        Page page = core.Set<Page>().IgnoreQueryFilters().FirstOrDefault(foundPage => foundPage.Id == seededContext.PageId);
        if (page is not null)
            await core.DeleteAsync(page);

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
    }

    private async Task<PageInfo> CreatePageInfoAsync(object payload)
    {
        using HttpResponseMessage response = await Client.PostAsJsonAsync(BaseUrl, payload);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return JsonSerializer.Deserialize<PageInfo>(content, JsonOptions)!;
    }

    private async Task<int> UpdatePageInfoAsync(int id, object payload)
    {
        using HttpResponseMessage response = await Client.PutAsJsonAsync($"{BaseUrl}({id})", payload);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return (int)response.StatusCode;
    }

    private async Task<int> PatchPageInfoAsync(int id, object payload)
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

    private async Task<int> DeletePageInfoAsync(int id)
    {
        using HttpResponseMessage response = await Client.DeleteAsync($"{BaseUrl}({id})");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return (int)response.StatusCode;
    }

    private async Task<PageInfo> GetPageInfoAsync(int id)
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}({id})");
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        string content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            return null;

        if (content.Contains("\"value\":[]", StringComparison.Ordinal))
            return null;

        return JsonSerializer.Deserialize<PageInfo>(content, JsonOptions);
    }

    private async Task<int> GetPageInfoCountAsync()
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}/$count");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return int.Parse(content);
    }

    private async Task<IReadOnlyList<PageInfo>> GetPageInfosAsync(int top)
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}?$top={top}");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return JsonSerializer.Deserialize<ODataEnvelope<PageInfo>>(content, JsonOptions)!.Value;
    }
    private async Task<int> GetPageInfoStatusCodeAsync(int id)
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}({id})");
        return (int)response.StatusCode;
    }
}








