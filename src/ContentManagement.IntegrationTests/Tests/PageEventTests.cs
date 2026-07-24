// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
    private static readonly JsonSerializerOptions JsonOptions = new(defaults: JsonSerializerDefaults.Web);

    private HttpClient Client { get; } = fixture.Client;

    private IServiceProvider Services =>
        fixture.Factory.Services;

    private static string Unique(string prefix) =>
        $"{prefix}-{Guid.NewGuid():N}";

    private async Task<int> SeedAppAsync()
    {
        using IServiceScope scope = Services.CreateScope();

        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        App app = new()
        {
            Name = Unique(prefix: "PageEventIntegration"),
            Domain = $"{Unique(prefix: "page")}.local",
            DefaultTheme = "Default",
            DefaultCultureId = string.Empty,
            TenantId = Unique(prefix: "tenant"),
            ConfigJson = "{}",
        };

        await core.Set<App>()
            .AddAsync(entity: app);

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
            Name = Unique(prefix: "Editors"),
            Description = "Integration role",
            Privs = string.Join(
                separator: ',',
                value:
                [
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
                    "pagerole_delete"
                ]),
        };

        await core.Set<Role>()
            .AddAsync(entity: role);

        await core.Set<UserRole>()
            .AddAsync(entity: new UserRole
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
            Name = Unique(prefix: "Landing"),
            Path = Unique(prefix: "landing"),
            Layout = "Default",
            ResourceKey = "Integration.Page",
            CreatedBy = "Guest",
            CreatedOn = DateTimeOffset.UtcNow,
            LastUpdatedBy = "Guest",
            LastUpdated = DateTimeOffset.UtcNow,
        };

        await core.Set<Page>()
            .AddAsync(entity: page);

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
requestUri: "/Api/Eventing",
value: new HttpEventMessage
{
    EventName = eventName,
    SSOUserId = "Guest",
    Data = JsonSerializer.Serialize(value: data, options: JsonOptions),
});

        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.Accepted, because: content);

        return response.StatusCode;
    }

    private static async Task WaitForAsync(Func<bool> condition, string because)
    {
        DateTimeOffset stopAt = DateTimeOffset.UtcNow.AddSeconds(seconds: 15);
        Exception lastException = null;

        while (DateTimeOffset.UtcNow < stopAt)
        {
            try
            {
                if (condition())
                {
                    return;
                }
            }
            catch (Exception exception)
            {
                lastException = exception;
            }

            await Task.Delay(millisecondsDelay: 100);
        }

        if (lastException is not null)
        {
            throw new TimeoutException(message: $"Timed out waiting because {because}.", innerException: lastException);
        }

        throw new TimeoutException(message: $"Timed out waiting because {because}.");
    }

    private bool HasPageInfo(int pageId, string title)
    {
        using IServiceScope scope = Services.CreateScope();

        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return core.Set<PageInfo>()
            .IgnoreQueryFilters()
            .Any(predicate: pageInfo => pageInfo.PageId == pageId && pageInfo.Title == title);
    }

    private bool HasContent(int pageId, string html)
    {
        using IServiceScope scope = Services.CreateScope();

        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return core.Set<Content>()
            .IgnoreQueryFilters()
            .Any(predicate: content => content.PageId == pageId && content.Html == html);
    }

    private bool HasPageRole(int pageId, Guid roleId)
    {
        using IServiceScope scope = Services.CreateScope();

        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return core.Set<PageRole>()
            .IgnoreQueryFilters()
            .Any(predicate: pageRole => pageRole.PageId == pageId && pageRole.RoleId == roleId);
    }

    private bool HasNoPageInfo(int pageId)
    {
        using IServiceScope scope = Services.CreateScope();

        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return !core.Set<PageInfo>()
            .IgnoreQueryFilters()
            .Any(predicate: pageInfo => pageInfo.PageId == pageId);
    }

    private bool HasNoContent(int pageId)
    {
        using IServiceScope scope = Services.CreateScope();

        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return !core.Set<Content>()
            .IgnoreQueryFilters()
            .Any(predicate: content => content.PageId == pageId);
    }

    private bool HasNoPageRole(int pageId)
    {
        using IServiceScope scope = Services.CreateScope();

        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return !core.Set<PageRole>()
            .IgnoreQueryFilters()
            .Any(predicate: pageRole => pageRole.PageId == pageId);
    }

    private async Task SeedPageInfoAsync(Page page, string title = "Landing")
    {
        await PostEventAsync(eventName: "page_add", data: CreatePageWithPageInfo(page: page, title: title));
        await WaitForAsync(condition: () => HasPageInfo(pageId: page.Id, title: title), because: "page_add should create the page info child row");
    }

    private async Task SeedContentAsync(Page page, string html = "<p>Landing body</p>")
    {
        await PostEventAsync(eventName: "page_add", data: CreatePageWithContent(page: page, html: html));
        await WaitForAsync(condition: () => HasContent(pageId: page.Id, html: html), because: "page_add should create the content child row");
    }

    private async Task SeedPageRoleAsync(Page page, Guid roleId)
    {
        await PostEventAsync(eventName: "page_add", data: CreatePageWithPageRole(page: page, roleId: roleId));
        await WaitForAsync(condition: () => HasPageRole(pageId: page.Id, roleId: roleId), because: "page_add should create the page role child row");
    }

    private async Task TeardownAppAsync(int appId)
    {
        using IServiceScope scope = Services.CreateScope();

        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        int[] pageIds =
        [
            .. core.Set<Page>()
            .IgnoreQueryFilters()
            .Where(predicate: page => page.AppId == appId)
            .Select(selector: page => page.Id)
        ];

        await core.Set<PageRole>()
            .IgnoreQueryFilters()
            .Where(predicate: pageRole => pageIds.Contains(value: pageRole.PageId))
            .ExecuteDeleteAsync();

        await core.Set<PageInfo>()
            .IgnoreQueryFilters()
            .Where(predicate: pageInfo => pageIds.Contains(value: pageInfo.PageId))
            .ExecuteDeleteAsync();

        await core.Set<Content>()
            .IgnoreQueryFilters()
            .Where(predicate: content => pageIds.Contains(value: content.PageId))
            .ExecuteDeleteAsync();

        await core.Set<Page>()
            .IgnoreQueryFilters()
            .Where(predicate: page => page.AppId == appId)
            .ExecuteDeleteAsync();

        Guid[] roleIds =
        [
            .. core.Set<Role>()
            .IgnoreQueryFilters()
            .Where(predicate: role => role.AppId == appId)
            .Select(selector: role => role.Id)
        ];

        await core.Set<UserRole>()
            .IgnoreQueryFilters()
            .Where(predicate: userRole => roleIds.Contains(value: userRole.RoleId))
            .ExecuteDeleteAsync();

        await core.Set<Role>()
            .IgnoreQueryFilters()
            .Where(predicate: role => role.AppId == appId)
            .ExecuteDeleteAsync();

        await core.Set<App>()
            .IgnoreQueryFilters()
            .Where(predicate: app => app.Id == appId)
            .ExecuteDeleteAsync();
    }
}