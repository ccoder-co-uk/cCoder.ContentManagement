// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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

    private static string Unique(string prefix) =>
        $"{prefix}-{Guid.NewGuid():N}";

    private sealed record SeededPageContext(int AppId, Guid RoleId, int LayoutId, string LayoutName);
    private sealed record ODataEnvelope<T>(List<T> Value);
    internal sealed record MenuResponse(bool Success);

    private Task<SeededPageContext> SeedDatabase(params string[] privileges)
        =>
        SeedDatabase(includeAppAdmin: true, privileges: privileges);

    private async Task<SeededPageContext> SeedDatabase(bool includeAppAdmin = true, params string[] privileges)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();

        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        App app = await core.AddAppAsync(app: new App
        {
            Name = Unique(prefix: "AcceptanceApp"),
            Domain = $"{Unique(prefix: "page")}.local",
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
            Privs = string.Join(
separator: ',',
values: privileges
                    .Concat(second: includeAppAdmin ? ["app_admin", "content_delete", "pageinfo_delete"] : ["content_delete", "pageinfo_delete"])
            .Distinct(comparer: StringComparer.OrdinalIgnoreCase)
            ),
        });

        await core.AddUserRoleAsync(userRole: new UserRole { RoleId = role.Id, UserId = "Guest" });

        Layout layout = await core.AddLayoutAsync(layout: new Layout
        {
            AppId = app.Id,
            Name = Unique(prefix: "Layout"),
            Description = "Acceptance layout",
            HeaderHtml = "<title>[page[title]]</title>",
            Html = "<main>[content[body]]</main>",
            Script = string.Empty,
        });

        return new SeededPageContext(AppId: app.Id, RoleId: role.Id, LayoutId: layout.Id, LayoutName: layout.Name);
    }

    private async Task<Page> CreatePageAsync(object payload)
    {
        using HttpResponseMessage response = await Client.PostAsJsonAsync(requestUri: BaseUrl, value: payload);
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return JsonSerializer.Deserialize<Page>(json: content, options: JsonOptions)!;
    }

    private object CreateValidPagePayload(
        SeededPageContext seededContext,
        string name,
        int order = 1,
        bool showOnMenus = true,
        string resourceKey = "Default",
        object parentId = null)
        =>
        new
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
        using HttpResponseMessage response = await Client.PutAsJsonAsync(requestUri: $"{BaseUrl}({id})", value: payload);
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return JsonSerializer.Deserialize<Page>(json: content, options: JsonOptions)!;
    }

    private async Task<Page> PatchPageAsync(int id, object payload)
    {
        using HttpRequestMessage request = new(method: HttpMethod.Patch, requestUri: $"{BaseUrl}({id})")
        {
            Content = JsonContent.Create(inputValue: payload),
        };

        using HttpResponseMessage response = await Client.SendAsync(request: request);
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return JsonSerializer.Deserialize<Page>(json: content, options: JsonOptions)!;
    }

    private async Task<int> DeletePageAsync(int id)
    {
        using HttpResponseMessage response = await Client.DeleteAsync(requestUri: $"{BaseUrl}({id})");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return (int)response.StatusCode;
    }

    private async Task<Page> GetPageAsync(int id)
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}({id})");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        string content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        if (content.Contains(value: "\"value\":[]", comparisonType: StringComparison.Ordinal))
        {
            return null;
        }

        return JsonSerializer.Deserialize<Page>(json: content, options: JsonOptions);
    }

    private async Task<int> GetPageCountAsync()
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}/$count");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return int.Parse(s: content);
    }

    private async Task<IReadOnlyList<Page>> GetPagesAsync(int top)
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}?$top={top}");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return JsonSerializer.Deserialize<ODataEnvelope<Page>>(json: content, options: JsonOptions)!.Value;
    }

    private async Task<JsonObject> GetPageQueryPayloadAsync(string queryString)
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}{queryString}");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return JsonNode.Parse(json: content)!.AsObject();
    }

    private async Task<Page> GetRootPageAsync(int id)
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}({id})/RootFor()");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        string content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return JsonSerializer.Deserialize<Page>(json: content, options: JsonOptions);
    }

    private async Task<MenuResponse> GetMenuAsync(int id, string culture = "")
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}({id})/Menu()?culture={Uri.EscapeDataString(stringToEscape: culture)}");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return JsonSerializer.Deserialize<MenuResponse>(json: content, options: JsonOptions)!;
    }

    private async Task<string> RenderPageAsync(int appId, string path, string theme = "Default", string culture = "")
    {
        using HttpResponseMessage response = await Client.GetAsync(
requestUri: $"{BaseUrl}/Render()?appId={appId}&path={Uri.EscapeDataString(stringToEscape: path)}&theme={Uri.EscapeDataString(stringToEscape: theme)}&culture={Uri.EscapeDataString(stringToEscape: culture)}");

        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return content;
    }

    private async Task Teardown(SeededPageContext seededContext)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();

        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        Content[] contents = core
            .Set<Content>()
            .IgnoreQueryFilters()
            .Where(predicate: content => content.Page.AppId == seededContext.AppId)
            .ToArray();

        if (contents.Length > 0)
        {
            await core.DeleteAllAsync(contents: contents);
        }

        PageInfo[] pageInfos = core
            .Set<PageInfo>()
            .IgnoreQueryFilters()
            .Where(predicate: pageInfo => pageInfo.Page.AppId == seededContext.AppId)
            .ToArray();

        if (pageInfos.Length > 0)
        {
            await core.DeleteAllAsync(pageInfos: pageInfos);
        }

        Page[] pages = core
            .Set<Page>()
            .IgnoreQueryFilters()
            .Where(predicate: page => page.AppId == seededContext.AppId)
            .ToArray();

        PageRole[] pageRoles = core
            .Set<PageRole>()
            .IgnoreQueryFilters()
            .Where(predicate: pageRole => pages.Select(selector: page => page.Id)
            .Contains(value: pageRole.PageId))
            .ToArray();

        if (pageRoles.Length > 0)
        {
            await core.DeleteAllAsync(pageRoles: pageRoles);
        }

        if (pages.Length > 0)
        {
            await core.DeleteAllAsync(pages: pages);
        }

        Layout layout = core.Set<Layout>()
            .IgnoreQueryFilters()
            .FirstOrDefault(predicate: foundLayout => foundLayout.Id == seededContext.LayoutId);

        if (layout is not null)
        {
            await core.DeleteAsync(layout: layout);
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
            .Set<PageInfo>()
            .IgnoreQueryFilters()
            .Any(predicate: pageInfo => pageInfo.PageId == pageId && pageInfo.CultureId == string.Empty);

        if (!hasDefaultInfo)
        {
            await core.AddPageInfoAsync(pageInfo: new PageInfo
            {
                PageId = pageId,
                CultureId = string.Empty,
                Title = title,
                Description = description,
                Keywords = keywords,
            });
        }

        bool hasContent = core
            .Set<Content>()
            .IgnoreQueryFilters()
            .Any(predicate: content => content.PageId == pageId && content.CultureId == string.Empty && content.Name == contentName);

        if (!hasContent)
        {
            await core.AddContentAsync(content: new Content
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
            .Set<PageInfo>()
            .IgnoreQueryFilters()
            .Where(predicate: pageInfo => pageInfo.PageId == pageId)
            .ToArray();

        if (pageInfos.Length > 0)
        {
            await core.DeleteAllAsync(pageInfos: pageInfos);
        }

        Content[] contents = core
            .Set<Content>()
            .IgnoreQueryFilters()
            .Where(predicate: content => content.PageId == pageId)
            .ToArray();

        if (contents.Length > 0)
        {
            await core.DeleteAllAsync(contents: contents);
        }
    }
    private async Task<int> GetPageStatusCodeAsync(int id)
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}({id})");
        return (int)response.StatusCode;
    }

    private PageInfo[] GetPageInfos(int pageId)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();

        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        return [.. core
            .Set<PageInfo>()
            .IgnoreQueryFilters()
            .Where(predicate: pageInfo => pageInfo.PageId == pageId)
            .OrderBy(keySelector: pageInfo => pageInfo.CultureId)];
    }

    private Page[] GetChildPages(int parentId)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();

        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        return [.. core
            .Set<Page>()
            .IgnoreQueryFilters()
            .Where(predicate: page => page.ParentId == parentId)
            .OrderBy(keySelector: page => page.Id)];
    }
}