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
public sealed partial class AppEventTests(ContentManagementIntegrationFixture fixture)
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
            Name = Unique(prefix: "ContentManagementIntegration"),
            Domain = $"{Unique(prefix: "content")}.local",
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

    private async Task SeedAppAdministratorAsync(int appId)
    {
        using IServiceScope scope = Services.CreateScope();

        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        Role role = new()
        {
            Id = Guid.NewGuid(),
            AppId = appId,
            Name = Unique(prefix: "Administrators"),
            Description = "Integration administrator role",
            Privs = string.Join(
                separator: ',',
                value:
                [
                    "app_admin",
                    "appculture_create",
                    "appculture_update",
                    "appculture_delete",
                    "component_create",
                    "component_update",
                    "component_delete",
                    "content_create",
                    "content_update",
                    "content_delete",
                    "layout_create",
                    "layout_update",
                    "layout_delete",
                    "page_create",
                    "page_update",
                    "page_delete",
                    "pageinfo_create",
                    "pageinfo_update",
                    "pageinfo_delete",
                    "pagerole_create",
                    "pagerole_update",
                    "pagerole_delete",
                    "resource_create",
                    "resource_update",
                    "resource_delete",
                    "script_create",
                    "script_update",
                    "script_delete",
                    "template_create",
                    "template_update",
                    "template_delete"
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
    }

    private async Task SeedCultureAsync(string cultureId, string name)
    {
        using IServiceScope scope = Services.CreateScope();

        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        if (await core.Set<Culture>()
            .IgnoreQueryFilters()
            .AnyAsync(predicate: culture => culture.Id == cultureId))
        {
            return;
        }

        await core.Set<Culture>()
            .AddAsync(entity: new Culture
            {
                Id = cultureId,
                Name = name,
            });

        await core.SaveChangesAsync();
    }

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

    private bool HasAppCulture(int appId, string cultureId = "en-GB")
    {
        using IServiceScope scope = Services.CreateScope();

        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return core.Set<AppCulture>()
            .IgnoreQueryFilters()
            .Any(predicate: item => item.AppId == appId && item.CultureId == cultureId);
    }

    private bool HasComponent(int appId)
    {
        using IServiceScope scope = Services.CreateScope();

        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return core.Set<Component>()
            .IgnoreQueryFilters()
            .Any(predicate: item => item.AppId == appId);
    }

    private bool HasLayout(int appId)
    {
        using IServiceScope scope = Services.CreateScope();

        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return core.Set<Layout>()
            .IgnoreQueryFilters()
            .Any(predicate: item => item.AppId == appId);
    }

    private bool HasPage(int appId)
    {
        using IServiceScope scope = Services.CreateScope();

        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return core.Set<Page>()
            .IgnoreQueryFilters()
            .Any(predicate: item => item.AppId == appId);
    }

    private bool HasResource(int appId)
    {
        using IServiceScope scope = Services.CreateScope();

        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return core.Set<Resource>()
            .IgnoreQueryFilters()
            .Any(predicate: item => item.AppId == appId);
    }

    private bool HasScript(int appId)
    {
        using IServiceScope scope = Services.CreateScope();

        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return core.Set<Script>()
            .IgnoreQueryFilters()
            .Any(predicate: item => item.AppId == appId);
    }

    private bool HasTemplate(int appId)
    {
        using IServiceScope scope = Services.CreateScope();

        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return core.Set<Template>()
            .IgnoreQueryFilters()
            .Any(predicate: item => item.AppId == appId);
    }

    private bool HasNoAppCulture(int appId)
    {
        using IServiceScope scope = Services.CreateScope();

        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return !core.Set<AppCulture>()
            .IgnoreQueryFilters()
            .Any(predicate: item => item.AppId == appId);
    }

    private bool HasNoComponent(int appId)
    {
        using IServiceScope scope = Services.CreateScope();

        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return !core.Set<Component>()
            .IgnoreQueryFilters()
            .Any(predicate: item => item.AppId == appId);
    }

    private bool HasNoLayout(int appId)
    {
        using IServiceScope scope = Services.CreateScope();

        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return !core.Set<Layout>()
            .IgnoreQueryFilters()
            .Any(predicate: item => item.AppId == appId);
    }

    private bool HasNoPage(int appId)
    {
        using IServiceScope scope = Services.CreateScope();

        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return !core.Set<Page>()
            .IgnoreQueryFilters()
            .Any(predicate: item => item.AppId == appId);
    }

    private bool HasNoResource(int appId)
    {
        using IServiceScope scope = Services.CreateScope();

        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return !core.Set<Resource>()
            .IgnoreQueryFilters()
            .Any(predicate: item => item.AppId == appId);
    }

    private bool HasNoScript(int appId)
    {
        using IServiceScope scope = Services.CreateScope();

        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return !core.Set<Script>()
            .IgnoreQueryFilters()
            .Any(predicate: item => item.AppId == appId);
    }

    private bool HasNoTemplate(int appId)
    {
        using IServiceScope scope = Services.CreateScope();

        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return !core.Set<Template>()
            .IgnoreQueryFilters()
            .Any(predicate: item => item.AppId == appId);
    }

    private App CreateAppWithAppCulture(int appId, string cultureId = "en-GB") =>
        new()
        {
            Id = appId,
            Cultures =
            [
                new AppCulture
                {
                    AppId = appId,
                    CultureId = cultureId,
                }
            ],
        };

    private App CreateAppWithComponent(int appId) =>
        new()
        {
            Id = appId,
            Components =
            [
                new Component
                {
                    AppId = appId,
                    Name = Unique(prefix: "Component"),
                    Description = "Integration component",
                    ResourceKey = "Integration.Component",
                    Key = Unique(prefix: "component"),
                    Content = "<p>Component</p>",
                    Script = string.Empty,
                    CreatedBy = "Guest",
                    CreatedOn = DateTimeOffset.UtcNow,
                    LastUpdatedBy = "Guest",
                    LastUpdated = DateTimeOffset.UtcNow,
                }
            ],
        };

    private App CreateAppWithLayout(int appId) =>
        new()
        {
            Id = appId,
            Layouts =
            [
                new Layout
                {
                    AppId = appId,
                    Name = Unique(prefix: "Layout"),
                    Description = "Integration layout",
                    HeaderHtml = string.Empty,
                    Html = "<main>[content[body]]</main>",
                    Script = string.Empty,
                    CreatedBy = "Guest",
                    CreatedOn = DateTimeOffset.UtcNow,
                    LastUpdatedBy = "Guest",
                    LastUpdated = DateTimeOffset.UtcNow,
                }
            ],
        };

    private App CreateAppWithPage(int appId) =>
        new()
        {
            Id = appId,
            Layouts =
            [
                new Layout
                {
                    AppId = appId,
                    Name = "Default",
                    Description = "Integration default layout",
                    HeaderHtml = string.Empty,
                    Html = "<main>[content[body]]</main>",
                    Script = string.Empty,
                    CreatedBy = "Guest",
                    CreatedOn = DateTimeOffset.UtcNow,
                    LastUpdatedBy = "Guest",
                    LastUpdated = DateTimeOffset.UtcNow,
                }
            ],
            Pages =
            [
                new Page
                {
                    AppId = appId,
                    Name = Unique(prefix: "Page"),
                    Path = Unique(prefix: "page"),
                    Layout = "Default",
                    ResourceKey = "Integration.Page",
                    CreatedBy = "Guest",
                    CreatedOn = DateTimeOffset.UtcNow,
                    LastUpdatedBy = "Guest",
                    LastUpdated = DateTimeOffset.UtcNow,
                    PageInfo =
                    [
                        new PageInfo
                        {
                            CultureId = string.Empty,
                            Title = "Integration Page",
                            Description = "Integration page",
                            Keywords = "integration,page",
                        }
                    ],
                    Contents =
                    [
                        new Content
                        {
                            CultureId = string.Empty,
                            Name = "body",
                            Html = "<p>Integration page</p>",
                        }
                    ],
                }
            ],
        };

    private App CreateAppWithResource(int appId) =>
        new()
        {
            Id = appId,
            Resources =
            [
                new Resource
                {
                    AppId = appId,
                    Name = Unique(prefix: "Resource"),
                    Description = "Integration resource",
                    Key = Unique(prefix: "resource"),
                    Culture = string.Empty,
                    DisplayName = "Integration Resource",
                    ShortDisplayName = "Resource",
                    CreatedBy = "Guest",
                    CreatedOn = DateTimeOffset.UtcNow,
                    LastUpdatedBy = "Guest",
                    LastUpdated = DateTimeOffset.UtcNow,
                }
            ],
        };

    private App CreateAppWithScript(int appId) =>
        new()
        {
            Id = appId,
            Scripts =
            [
                new Script
                {
                    AppId = appId,
                    Name = Unique(prefix: "Script"),
                    Description = "Integration script",
                    Key = Unique(prefix: "script"),
                    Content = "console.log('integration');",
                    CreatedBy = "Guest",
                    CreatedOn = DateTimeOffset.UtcNow,
                    LastUpdatedBy = "Guest",
                    LastUpdated = DateTimeOffset.UtcNow,
                }
            ],
        };

    private App CreateAppWithTemplate(int appId) =>
        new()
        {
            Id = appId,
            Templates =
            [
                new Template
                {
                    AppId = appId,
                    Name = Unique(prefix: "Template"),
                    Description = "Integration template",
                    ResourceKey = "Integration.Template",
                    RawString = "<p>Template</p>",
                    CreatedBy = "Guest",
                    CreatedOn = DateTimeOffset.UtcNow,
                    LastUpdatedBy = "Guest",
                    LastUpdated = DateTimeOffset.UtcNow,
                }
            ],
        };

    private async Task SeedAppCultureAsync(int appId)
    {
        await SeedCultureAsync(cultureId: "en-GB", name: "English (UK)");
        await PostEventAsync(eventName: "app_add", data: CreateAppWithAppCulture(appId: appId));
        await WaitForAsync(condition: () => HasAppCulture(appId: appId), because: "app_add should create the app culture child row");
    }

    private async Task SeedComponentAsync(int appId)
    {
        await PostEventAsync(eventName: "app_add", data: CreateAppWithComponent(appId: appId));
        await WaitForAsync(condition: () => HasComponent(appId: appId), because: "app_add should create the component child row");
    }

    private async Task SeedLayoutAsync(int appId)
    {
        await PostEventAsync(eventName: "app_add", data: CreateAppWithLayout(appId: appId));
        await WaitForAsync(condition: () => HasLayout(appId: appId), because: "app_add should create the layout child row");
    }

    private async Task SeedPageAsync(int appId)
    {
        await PostEventAsync(eventName: "app_add", data: CreateAppWithPage(appId: appId));
        await WaitForAsync(condition: () => HasPage(appId: appId), because: "app_add should create the page child row");
    }

    private async Task SeedResourceAsync(int appId)
    {
        await PostEventAsync(eventName: "app_add", data: CreateAppWithResource(appId: appId));
        await WaitForAsync(condition: () => HasResource(appId: appId), because: "app_add should create the resource child row");
    }

    private async Task SeedScriptAsync(int appId)
    {
        await PostEventAsync(eventName: "app_add", data: CreateAppWithScript(appId: appId));
        await WaitForAsync(condition: () => HasScript(appId: appId), because: "app_add should create the script child row");
    }

    private async Task SeedTemplateAsync(int appId)
    {
        await PostEventAsync(eventName: "app_add", data: CreateAppWithTemplate(appId: appId));
        await WaitForAsync(condition: () => HasTemplate(appId: appId), because: "app_add should create the template child row");
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

        await core.Set<PageInfo>()
            .IgnoreQueryFilters()
            .Where(predicate: pageInfo => pageIds.Contains(value: pageInfo.PageId))
            .ExecuteDeleteAsync();

        await core.Set<Content>()
            .IgnoreQueryFilters()
            .Where(predicate: content => pageIds.Contains(value: content.PageId))
            .ExecuteDeleteAsync();

        await core.Set<PageRole>()
            .IgnoreQueryFilters()
            .Where(predicate: pageRole => pageIds.Contains(value: pageRole.PageId))
            .ExecuteDeleteAsync();

        await core.Set<Page>()
            .IgnoreQueryFilters()
            .Where(predicate: page => page.AppId == appId)
            .ExecuteDeleteAsync();

        await core.Set<AppCulture>()
            .IgnoreQueryFilters()
            .Where(predicate: culture => culture.AppId == appId)
            .ExecuteDeleteAsync();

        await core.Set<Component>()
            .IgnoreQueryFilters()
            .Where(predicate: component => component.AppId == appId)
            .ExecuteDeleteAsync();

        await core.Set<Layout>()
            .IgnoreQueryFilters()
            .Where(predicate: layout => layout.AppId == appId)
            .ExecuteDeleteAsync();

        await core.Set<Resource>()
            .IgnoreQueryFilters()
            .Where(predicate: resource => resource.AppId == appId)
            .ExecuteDeleteAsync();

        await core.Set<Script>()
            .IgnoreQueryFilters()
            .Where(predicate: script => script.AppId == appId)
            .ExecuteDeleteAsync();

        await core.Set<Template>()
            .IgnoreQueryFilters()
            .Where(predicate: template => template.AppId == appId)
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