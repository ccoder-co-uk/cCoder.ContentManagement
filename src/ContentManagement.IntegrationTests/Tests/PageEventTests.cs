using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using cCoder.Data;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Eventing.Http.Models;
using ContentManagement.IntegrationTests.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using PageRole = cCoder.Data.Models.Security.PageRole;

namespace ContentManagement.IntegrationTests.Tests;

[Collection(ContentManagementIntegrationCollection.Name)]
public sealed partial class PageEventTests(ContentManagementIntegrationFixture fixture)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private HttpClient Client { get; } = fixture.Client;

    private IServiceProvider Services => fixture.Factory.Services;

    private static string Unique(string prefix) =>
        $"{prefix}-{Guid.NewGuid():N}";

    private async Task<int> SeedAppAsync()
    {
        using IServiceScope scope = Services.CreateScope();
        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        App app = new()
        {
            Name = Unique("PageEventIntegration"),
            Domain = $"{Unique("page")}.local",
            DefaultTheme = "Default",
            DefaultCultureId = string.Empty,
            TenantId = Unique("tenant"),
            ConfigJson = "{}",
        };

        await core.Set<App>().AddAsync(app);
        await core.SaveChangesAsync();
        return app.Id;
    }

    private async Task<Guid> SeedRoleAsync(int appId)
    {
        using IServiceScope scope = Services.CreateScope();
        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        Role role = new()
        {
            Id = Guid.NewGuid(),
            AppId = appId,
            Name = Unique("Editors"),
            Description = "Integration role",
            Privs = string.Join(
                ',',
                "app_admin",
                "content_create",
                "content_update",
                "content_delete",
                "page_create",
                "page_update",
                "page_delete",
                "pageinfo_create",
                "pageinfo_update",
                "pageinfo_delete",
                "pagerole_create",
                "pagerole_update",
                "pagerole_delete"),
        };

        await core.Set<Role>().AddAsync(role);
        await core.Set<UserRole>().AddAsync(new UserRole
        {
            RoleId = role.Id,
            UserId = "Guest",
        });
        await core.SaveChangesAsync();
        return role.Id;
    }

    private async Task<Page> SeedPageAsync(int appId)
    {
        using IServiceScope scope = Services.CreateScope();
        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        Page page = new()
        {
            AppId = appId,
            Name = Unique("Landing"),
            Path = Unique("landing"),
            Layout = "Default",
            ResourceKey = "Integration.Page",
            CreatedBy = "Guest",
            CreatedOn = DateTimeOffset.UtcNow,
            LastUpdatedBy = "Guest",
            LastUpdated = DateTimeOffset.UtcNow,
        };

        await core.Set<Page>().AddAsync(page);
        await core.SaveChangesAsync();
        return page;
    }

    private Page CreatePageWithPageInfo(
        Page page,
        string title) =>
        new()
        {
            Id = page.Id,
            AppId = page.AppId,
            Name = page.Name,
            Path = page.Path,
            Layout = page.Layout,
            ResourceKey = page.ResourceKey,
            CreatedBy = page.CreatedBy,
            CreatedOn = page.CreatedOn,
            LastUpdatedBy = "Guest",
            LastUpdated = DateTimeOffset.UtcNow,
            PageInfo =
            [
                new PageInfo
                {
                    PageId = page.Id,
                    CultureId = string.Empty,
                    Title = title,
                    Description = $"{title} description",
                    Keywords = "page,integration",
                }
            ],
        };

    private Page CreatePageWithContent(
        Page page,
        string html) =>
        new()
        {
            Id = page.Id,
            AppId = page.AppId,
            Name = page.Name,
            Path = page.Path,
            Layout = page.Layout,
            ResourceKey = page.ResourceKey,
            CreatedBy = page.CreatedBy,
            CreatedOn = page.CreatedOn,
            LastUpdatedBy = "Guest",
            LastUpdated = DateTimeOffset.UtcNow,
            Contents =
            [
                new Content
                {
                    PageId = page.Id,
                    CultureId = string.Empty,
                    Name = "body",
                    Html = html,
                }
            ],
        };

    private Page CreatePageWithPageRole(
        Page page,
        Guid roleId) =>
        new()
        {
            Id = page.Id,
            AppId = page.AppId,
            Name = page.Name,
            Path = page.Path,
            Layout = page.Layout,
            ResourceKey = page.ResourceKey,
            CreatedBy = page.CreatedBy,
            CreatedOn = page.CreatedOn,
            LastUpdatedBy = "Guest",
            LastUpdated = DateTimeOffset.UtcNow,
            Roles =
            [
                new PageRole
                {
                    PageId = page.Id,
                    RoleId = roleId,
                }
            ],
        };

    private async Task<HttpStatusCode> PostEventAsync(string eventName, object data)
    {
        using HttpResponseMessage response = await Client.PostAsJsonAsync(
            "/Api/Eventing",
            new HttpEventMessage
            {
                EventName = eventName,
                SSOUserId = "Guest",
                Data = JsonSerializer.Serialize(data, JsonOptions),
            });

        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.Accepted, content);
        return response.StatusCode;
    }

    private static async Task WaitForAsync(Func<bool> condition, string because)
    {
        DateTimeOffset stopAt = DateTimeOffset.UtcNow.AddSeconds(15);
        Exception lastException = null;

        while (DateTimeOffset.UtcNow < stopAt)
        {
            try
            {
                if (condition())
                    return;
            }
            catch (Exception exception)
            {
                lastException = exception;
            }

            await Task.Delay(100);
        }

        if (lastException is not null)
            throw new TimeoutException($"Timed out waiting because {because}.", lastException);

        throw new TimeoutException($"Timed out waiting because {because}.");
    }

    private bool HasPageInfo(int pageId, string title)
    {
        using IServiceScope scope = Services.CreateScope();
        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return core.Set<PageInfo>().IgnoreQueryFilters()
            .Any(pageInfo => pageInfo.PageId == pageId && pageInfo.Title == title);
    }

    private bool HasContent(int pageId, string html)
    {
        using IServiceScope scope = Services.CreateScope();
        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return core.Set<Content>().IgnoreQueryFilters()
            .Any(content => content.PageId == pageId && content.Html == html);
    }

    private bool HasPageRole(int pageId, Guid roleId)
    {
        using IServiceScope scope = Services.CreateScope();
        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return core.Set<PageRole>().IgnoreQueryFilters()
            .Any(pageRole => pageRole.PageId == pageId && pageRole.RoleId == roleId);
    }

    private bool HasNoPageInfo(int pageId)
    {
        using IServiceScope scope = Services.CreateScope();
        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return !core.Set<PageInfo>().IgnoreQueryFilters().Any(pageInfo => pageInfo.PageId == pageId);
    }

    private bool HasNoContent(int pageId)
    {
        using IServiceScope scope = Services.CreateScope();
        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return !core.Set<Content>().IgnoreQueryFilters().Any(content => content.PageId == pageId);
    }

    private bool HasNoPageRole(int pageId)
    {
        using IServiceScope scope = Services.CreateScope();
        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return !core.Set<PageRole>().IgnoreQueryFilters().Any(pageRole => pageRole.PageId == pageId);
    }

    private async Task SeedPageInfoAsync(Page page, string title = "Landing")
    {
        await PostEventAsync("page_add", CreatePageWithPageInfo(page, title));
        await WaitForAsync(() => HasPageInfo(page.Id, title), "page_add should create the page info child row");
    }

    private async Task SeedContentAsync(Page page, string html = "<p>Landing body</p>")
    {
        await PostEventAsync("page_add", CreatePageWithContent(page, html));
        await WaitForAsync(() => HasContent(page.Id, html), "page_add should create the content child row");
    }

    private async Task SeedPageRoleAsync(Page page, Guid roleId)
    {
        await PostEventAsync("page_add", CreatePageWithPageRole(page, roleId));
        await WaitForAsync(() => HasPageRole(page.Id, roleId), "page_add should create the page role child row");
    }

    private async Task TeardownAppAsync(int appId)
    {
        using IServiceScope scope = Services.CreateScope();
        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        int[] pageIds =
        [
            .. core.Set<Page>().IgnoreQueryFilters()
                .Where(page => page.AppId == appId)
                .Select(page => page.Id)
        ];

        await core.Set<PageRole>().IgnoreQueryFilters()
            .Where(role => pageIds.Contains(role.PageId))
            .ExecuteDeleteAsync();
        await core.Set<PageInfo>().IgnoreQueryFilters()
            .Where(pageInfo => pageIds.Contains(pageInfo.PageId))
            .ExecuteDeleteAsync();
        await core.Set<Content>().IgnoreQueryFilters()
            .Where(content => pageIds.Contains(content.PageId))
            .ExecuteDeleteAsync();
        await core.Set<Page>().IgnoreQueryFilters()
            .Where(page => page.AppId == appId)
            .ExecuteDeleteAsync();
        Guid[] roleIds =
        [
            .. core.Set<Role>().IgnoreQueryFilters()
                .Where(role => role.AppId == appId)
                .Select(role => role.Id)
        ];

        await core.Set<UserRole>().IgnoreQueryFilters()
            .Where(userRole => roleIds.Contains(userRole.RoleId))
            .ExecuteDeleteAsync();
        await core.Set<Role>().IgnoreQueryFilters()
            .Where(role => role.AppId == appId)
            .ExecuteDeleteAsync();
        await core.Set<App>().IgnoreQueryFilters()
            .Where(app => app.Id == appId)
            .ExecuteDeleteAsync();
    }
}
