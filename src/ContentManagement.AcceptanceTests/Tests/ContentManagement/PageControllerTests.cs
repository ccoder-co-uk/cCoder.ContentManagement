using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
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
public sealed partial class PageControllerTests(WebAcceptanceFixture fixture)
{
    private HttpClient Client { get; } = fixture.Client;
    private string BaseUrl { get; } = "/Api/Core/Page";
    private static JsonSerializerOptions JsonOptions { get; } = new() { PropertyNameCaseInsensitive = true };

    private static string Unique(string prefix) => $"{prefix}-{Guid.NewGuid():N}";

    private sealed record SeededPageContext(int AppId, Guid RoleId, int LayoutId, string LayoutName);
    private sealed record ODataEnvelope<T>(List<T> Value);
    internal sealed record MenuResponse(bool Success);

    private Task<SeededPageContext> SeedDatabase(params string[] privileges)
        => SeedDatabase(includeAppAdmin: true, privileges);

    private async Task<SeededPageContext> SeedDatabase(bool includeAppAdmin = true, params string[] privileges)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        App app = await core.AddAppAsync(new App
        {
            Name = Unique("AcceptanceApp"),
            Domain = $"{Unique("page")}.local",
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
            Privs = string.Join(
                ',',
                privileges
                    .Concat(includeAppAdmin ? ["app_admin", "content_delete", "pageinfo_delete"] : ["content_delete", "pageinfo_delete"])
                    .Distinct(StringComparer.OrdinalIgnoreCase)
            ),
        });

        await core.AddUserRoleAsync(new UserRole { RoleId = role.Id, UserId = "Guest" });

        Layout layout = await core.AddLayoutAsync(new Layout
        {
            AppId = app.Id,
            Name = Unique("Layout"),
            Description = "Acceptance layout",
            HeaderHtml = "<title>[page[title]]</title>",
            Html = "<main>[content[body]]</main>",
            Script = string.Empty,
        });

        return new SeededPageContext(app.Id, role.Id, layout.Id, layout.Name);
    }

    private async Task<Page> CreatePageAsync(object payload)
    {
        using HttpResponseMessage response = await Client.PostAsJsonAsync(BaseUrl, payload);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return JsonSerializer.Deserialize<Page>(content, JsonOptions)!;
    }

    private object CreateValidPagePayload(
        SeededPageContext seededContext,
        string name,
        int order = 1,
        bool showOnMenus = true,
        string resourceKey = "Default",
        object parentId = null)
        => new
        {
            appId = seededContext.AppId,
            parentId,
            name,
            order,
            showOnMenus,
            resourceKey,
            layout = seededContext.LayoutName,
            pageInfo = new[]
            {
                new
                {
                    cultureId = "",
                    title = name,
                    description = $"{name} description",
                    keywords = $"{name.ToLowerInvariant()},acceptance",
                },
            },
            contents = new[]
            {
                new
                {
                    cultureId = "",
                    name = "body",
                    html = $"<p>{name} body</p>",
                },
            },
        };

    private async Task<Page> UpdatePageAsync(int id, object payload)
    {
        using HttpResponseMessage response = await Client.PutAsJsonAsync($"{BaseUrl}({id})", payload);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return JsonSerializer.Deserialize<Page>(content, JsonOptions)!;
    }

    private async Task<Page> PatchPageAsync(int id, object payload)
    {
        using HttpRequestMessage request = new(HttpMethod.Patch, $"{BaseUrl}({id})")
        {
            Content = JsonContent.Create(payload),
        };

        using HttpResponseMessage response = await Client.SendAsync(request);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return JsonSerializer.Deserialize<Page>(content, JsonOptions)!;
    }

    private async Task<int> DeletePageAsync(int id)
    {
        using HttpResponseMessage response = await Client.DeleteAsync($"{BaseUrl}({id})");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return (int)response.StatusCode;
    }

    private async Task<Page> GetPageAsync(int id)
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}({id})");
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        string content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            return null;

        if (content.Contains("\"value\":[]", StringComparison.Ordinal))
            return null;

        return JsonSerializer.Deserialize<Page>(content, JsonOptions);
    }

    private async Task<int> GetPageCountAsync()
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}/$count");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return int.Parse(content);
    }

    private async Task<IReadOnlyList<Page>> GetPagesAsync(int top)
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}?$top={top}");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return JsonSerializer.Deserialize<ODataEnvelope<Page>>(content, JsonOptions)!.Value;
    }

    private async Task<JsonObject> GetPageQueryPayloadAsync(string queryString)
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}{queryString}");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return JsonNode.Parse(content)!.AsObject();
    }

    private async Task<Page> GetRootPageAsync(int id)
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}({id})/RootFor()");
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        string content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            return null;

        return JsonSerializer.Deserialize<Page>(content, JsonOptions);
    }

    private async Task<MenuResponse> GetMenuAsync(int id, string culture = "")
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}({id})/Menu()?culture={Uri.EscapeDataString(culture)}");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return JsonSerializer.Deserialize<MenuResponse>(content, JsonOptions)!;
    }

    private async Task<string> RenderPageAsync(int appId, string path, string theme = "Default", string culture = "")
    {
        using HttpResponseMessage response = await Client.GetAsync(
            $"{BaseUrl}/Render()?appId={appId}&path={Uri.EscapeDataString(path)}&theme={Uri.EscapeDataString(theme)}&culture={Uri.EscapeDataString(culture)}");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return content;
    }

    private async Task Teardown(SeededPageContext seededContext)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        Content[] contents = core
            .Set<Content>().IgnoreQueryFilters()
            .Where(content => content.Page.AppId == seededContext.AppId)
            .ToArray();

        if (contents.Length > 0)
            await core.DeleteAllAsync(contents);

        PageInfo[] pageInfos = core
            .Set<PageInfo>().IgnoreQueryFilters()
            .Where(pageInfo => pageInfo.Page.AppId == seededContext.AppId)
            .ToArray();

        if (pageInfos.Length > 0)
            await core.DeleteAllAsync(pageInfos);

        Page[] pages = core
            .Set<Page>().IgnoreQueryFilters()
            .Where(page => page.AppId == seededContext.AppId)
            .ToArray();

        PageRole[] pageRoles = core
            .Set<PageRole>().IgnoreQueryFilters()
            .Where(pageRole => pages.Select(page => page.Id).Contains(pageRole.PageId))
            .ToArray();

        if (pageRoles.Length > 0)
            await core.DeleteAllAsync(pageRoles);

        if (pages.Length > 0)
            await core.DeleteAllAsync(pages);

        Layout layout = core.Set<Layout>().IgnoreQueryFilters().FirstOrDefault(foundLayout => foundLayout.Id == seededContext.LayoutId);
        if (layout is not null)
            await core.DeleteAsync(layout);

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

    private async Task EnsurePageChildrenAsync(
        int pageId,
        string title,
        string description,
        string keywords,
        string contentName,
        string html)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        bool hasDefaultInfo = core
            .Set<PageInfo>().IgnoreQueryFilters()
            .Any(pageInfo => pageInfo.PageId == pageId && pageInfo.CultureId == string.Empty);

        if (!hasDefaultInfo)
        {
            await core.AddPageInfoAsync(new PageInfo
            {
                PageId = pageId,
                CultureId = string.Empty,
                Title = title,
                Description = description,
                Keywords = keywords,
            });
        }

        bool hasContent = core
            .Set<Content>().IgnoreQueryFilters()
            .Any(content => content.PageId == pageId && content.CultureId == string.Empty && content.Name == contentName);

        if (!hasContent)
        {
            await core.AddContentAsync(new Content
            {
                PageId = pageId,
                CultureId = string.Empty,
                Name = contentName,
                Html = html,
            });
        }
    }

    private async Task DeletePageChildrenAsync(int pageId)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        PageInfo[] pageInfos = core
            .Set<PageInfo>().IgnoreQueryFilters()
            .Where(pageInfo => pageInfo.PageId == pageId)
            .ToArray();

        if (pageInfos.Length > 0)
            await core.DeleteAllAsync(pageInfos);

        Content[] contents = core
            .Set<Content>().IgnoreQueryFilters()
            .Where(content => content.PageId == pageId)
            .ToArray();

        if (contents.Length > 0)
            await core.DeleteAllAsync(contents);
    }
    private async Task<int> GetPageStatusCodeAsync(int id)
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}({id})");
        return (int)response.StatusCode;
    }

    private PageInfo[] GetPageInfos(int pageId)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        return [.. core
            .Set<PageInfo>().IgnoreQueryFilters()
            .Where(pageInfo => pageInfo.PageId == pageId)
            .OrderBy(pageInfo => pageInfo.CultureId)];
    }

    private Page[] GetChildPages(int parentId)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        return [.. core
            .Set<Page>().IgnoreQueryFilters()
            .Where(page => page.ParentId == parentId)
            .OrderBy(page => page.Id)];
    }
}








