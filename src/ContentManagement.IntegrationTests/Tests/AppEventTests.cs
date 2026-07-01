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
            Name = Unique("ContentManagementIntegration"),
            Domain = $"{Unique("content")}.local",
            DefaultTheme = "Default",
            DefaultCultureId = string.Empty,
            TenantId = Unique("tenant"),
            ConfigJson = "{}",
        };

        await core.Set<App>().AddAsync(app);
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
            Name = Unique("Administrators"),
            Description = "Integration administrator role",
            Privs = string.Join(
                ',',
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
                "template_delete"),
        };

        await core.Set<Role>().AddAsync(role);
        await core.Set<UserRole>().AddAsync(new UserRole
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

        if (await core.Set<Culture>().IgnoreQueryFilters().AnyAsync(culture => culture.Id == cultureId))
            return;

        await core.Set<Culture>().AddAsync(new Culture
        {
            Id = cultureId,
            Name = name,
        });

        await core.SaveChangesAsync();
    }

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

    private bool HasAppCulture(int appId, string cultureId = "en-GB")
    {
        using IServiceScope scope = Services.CreateScope();
        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return core.Set<AppCulture>().IgnoreQueryFilters()
            .Any(item => item.AppId == appId && item.CultureId == cultureId);
    }

    private bool HasComponent(int appId)
    {
        using IServiceScope scope = Services.CreateScope();
        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return core.Set<Component>().IgnoreQueryFilters().Any(item => item.AppId == appId);
    }

    private bool HasLayout(int appId)
    {
        using IServiceScope scope = Services.CreateScope();
        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return core.Set<Layout>().IgnoreQueryFilters().Any(item => item.AppId == appId);
    }

    private bool HasPage(int appId)
    {
        using IServiceScope scope = Services.CreateScope();
        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return core.Set<Page>().IgnoreQueryFilters().Any(item => item.AppId == appId);
    }

    private bool HasResource(int appId)
    {
        using IServiceScope scope = Services.CreateScope();
        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return core.Set<Resource>().IgnoreQueryFilters().Any(item => item.AppId == appId);
    }

    private bool HasScript(int appId)
    {
        using IServiceScope scope = Services.CreateScope();
        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return core.Set<Script>().IgnoreQueryFilters().Any(item => item.AppId == appId);
    }

    private bool HasTemplate(int appId)
    {
        using IServiceScope scope = Services.CreateScope();
        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return core.Set<Template>().IgnoreQueryFilters().Any(item => item.AppId == appId);
    }

    private bool HasNoAppCulture(int appId)
    {
        using IServiceScope scope = Services.CreateScope();
        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return !core.Set<AppCulture>().IgnoreQueryFilters().Any(item => item.AppId == appId);
    }

    private bool HasNoComponent(int appId)
    {
        using IServiceScope scope = Services.CreateScope();
        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return !core.Set<Component>().IgnoreQueryFilters().Any(item => item.AppId == appId);
    }

    private bool HasNoLayout(int appId)
    {
        using IServiceScope scope = Services.CreateScope();
        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return !core.Set<Layout>().IgnoreQueryFilters().Any(item => item.AppId == appId);
    }

    private bool HasNoPage(int appId)
    {
        using IServiceScope scope = Services.CreateScope();
        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return !core.Set<Page>().IgnoreQueryFilters().Any(item => item.AppId == appId);
    }

    private bool HasNoResource(int appId)
    {
        using IServiceScope scope = Services.CreateScope();
        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return !core.Set<Resource>().IgnoreQueryFilters().Any(item => item.AppId == appId);
    }

    private bool HasNoScript(int appId)
    {
        using IServiceScope scope = Services.CreateScope();
        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return !core.Set<Script>().IgnoreQueryFilters().Any(item => item.AppId == appId);
    }

    private bool HasNoTemplate(int appId)
    {
        using IServiceScope scope = Services.CreateScope();
        using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return !core.Set<Template>().IgnoreQueryFilters().Any(item => item.AppId == appId);
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
                    Name = Unique("Component"),
                    Description = "Integration component",
                    ResourceKey = "Integration.Component",
                    Key = Unique("component"),
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
                    Name = Unique("Layout"),
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
            Pages =
            [
                new Page
                {
                    AppId = appId,
                    Name = Unique("Page"),
                    Path = Unique("page"),
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
                    Name = Unique("Resource"),
                    Description = "Integration resource",
                    Key = Unique("resource"),
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
                    Name = Unique("Script"),
                    Description = "Integration script",
                    Key = Unique("script"),
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
                    Name = Unique("Template"),
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
        await SeedCultureAsync("en-GB", "English (UK)");
        await PostEventAsync("app_add", CreateAppWithAppCulture(appId));
        await WaitForAsync(() => HasAppCulture(appId), "app_add should create the app culture child row");
    }

    private async Task SeedComponentAsync(int appId)
    {
        await PostEventAsync("app_add", CreateAppWithComponent(appId));
        await WaitForAsync(() => HasComponent(appId), "app_add should create the component child row");
    }

    private async Task SeedLayoutAsync(int appId)
    {
        await PostEventAsync("app_add", CreateAppWithLayout(appId));
        await WaitForAsync(() => HasLayout(appId), "app_add should create the layout child row");
    }

    private async Task SeedPageAsync(int appId)
    {
        await PostEventAsync("app_add", CreateAppWithPage(appId));
        await WaitForAsync(() => HasPage(appId), "app_add should create the page child row");
    }

    private async Task SeedResourceAsync(int appId)
    {
        await PostEventAsync("app_add", CreateAppWithResource(appId));
        await WaitForAsync(() => HasResource(appId), "app_add should create the resource child row");
    }

    private async Task SeedScriptAsync(int appId)
    {
        await PostEventAsync("app_add", CreateAppWithScript(appId));
        await WaitForAsync(() => HasScript(appId), "app_add should create the script child row");
    }

    private async Task SeedTemplateAsync(int appId)
    {
        await PostEventAsync("app_add", CreateAppWithTemplate(appId));
        await WaitForAsync(() => HasTemplate(appId), "app_add should create the template child row");
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
        await core.Set<AppCulture>().IgnoreQueryFilters()
            .Where(culture => culture.AppId == appId)
            .ExecuteDeleteAsync();
        await core.Set<Component>().IgnoreQueryFilters()
            .Where(component => component.AppId == appId)
            .ExecuteDeleteAsync();
        await core.Set<Layout>().IgnoreQueryFilters()
            .Where(layout => layout.AppId == appId)
            .ExecuteDeleteAsync();
        await core.Set<Resource>().IgnoreQueryFilters()
            .Where(resource => resource.AppId == appId)
            .ExecuteDeleteAsync();
        await core.Set<Script>().IgnoreQueryFilters()
            .Where(script => script.AppId == appId)
            .ExecuteDeleteAsync();
        await core.Set<Template>().IgnoreQueryFilters()
            .Where(template => template.AppId == appId)
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
