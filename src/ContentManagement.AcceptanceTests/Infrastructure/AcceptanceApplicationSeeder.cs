// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ContentMetadataCache = cCoder.ContentManagement.Exposures.Caching.IMetadataCache;

namespace Web.AcceptanceTests.Infrastructure;

internal sealed class AcceptanceApplicationSeeder(IServiceProvider services)
{
    private const int AppId = 1;
    private const string AppDomain = "localhost";
    private const string AcceptanceAdminRoleName = "Acceptance Administrators";
    private const string AcceptanceAdminPrivileges =
        "app_admin,"
        + "app_create,app_read,app_update,app_delete,"
        + "appculture_create,appculture_read,appculture_update,appculture_delete,"
        + "commonobject_create,commonobject_update,commonobject_delete,"
        + "component_create,component_read,component_update,component_delete,"
        + "culture_create,culture_update,culture_delete,"
        + "layout_create,layout_read,layout_update,layout_delete,"
        + "package_create,package_update,package_delete,"
        + "packageitem_create,packageitem_update,packageitem_delete,"
        + "page_create,page_read,page_update,page_delete,"
        + "pageinfo_create,pageinfo_read,pageinfo_update,pageinfo_delete,"
        + "pagerole_create,pagerole_read,pagerole_update,pagerole_delete,"
        + "resource_create,resource_read,resource_update,resource_delete,"
        + "script_create,script_read,script_update,script_delete,"
        + "submission_create,submission_read,submission_update,submission_delete,"
        + "template_create,template_read,template_update,template_delete";

    public async Task SeedAsync()
    {
        using IServiceScope scope = services.CreateScope();

        using DbContext core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        await EnsureAppAsync(core: core);
        await EnsureGuestUserAsync(core: core);
        await EnsureGuestAdminAsync(core: core);
        await SeedCapturedAppDataAsync(core: core);
        await SeedCommonObjectsAsync(core: core);
        RefreshCaches(services: scope.ServiceProvider);
    }

    private static async Task EnsureAppAsync(DbContext core)
    {
        if (await core.Set<App>()
            .AnyAsync(predicate: app => app.Id == AppId))
        {
            return;
        }

        core.Add(entity: new App
        {
            Name = "Acceptance",
            Domain = AppDomain,
            DefaultTheme = "Default",
            DefaultCultureId = string.Empty,
            TenantId = "acceptance",
            ConfigJson = AcceptanceAssetLoader.LoadText(fileName: "DefaultAppConfig.json"),
        });

        await core.SaveChangesAsync();
    }

    private static async Task EnsureGuestAdminAsync(DbContext core)
    {
        Role role = await core.Set<Role>()
            .FirstOrDefaultAsync(predicate: existing =>
            existing.AppId == AppId && existing.Name == AcceptanceAdminRoleName);

        if (role is null)
        {
            role = new Role
            {
                Id = Guid.NewGuid(),
                AppId = AppId,
                Name = AcceptanceAdminRoleName,
                Description = "Acceptance bootstrap role",
                Privs = AcceptanceAdminPrivileges,
            };

            core.Add(entity: role);
            await core.SaveChangesAsync();
        }
        else
        {
            if (role.Privs != AcceptanceAdminPrivileges)
            {
                role.Privs = AcceptanceAdminPrivileges;
                await core.SaveChangesAsync();
            }
        }

        bool hasGuestRole = await core.Set<UserRole>()
            .AnyAsync(predicate: existing =>
            existing.RoleId == role.Id && existing.UserId == "Guest");

        if (!hasGuestRole)
        {
            core.Add(entity: new UserRole { RoleId = role.Id, UserId = "Guest" });
            await core.SaveChangesAsync();
        }
    }

    private static async Task EnsureGuestUserAsync(DbContext core)
    {
        if (await core.Set<User>()
            .AnyAsync(predicate: existing => existing.Id == "Guest"))
        {
            return;
        }

        core.Add(entity: new User
        {
            Id = "Guest",
            DefaultCultureId = string.Empty,
            DisplayName = "Guest",
            Email = string.Empty,
            IsActive = true,
        });

        await core.SaveChangesAsync();
    }

    private static async Task SeedCapturedAppDataAsync(DbContext core)
    {
        await SeedLayoutsAsync(core: core);
        await SeedTemplatesAsync(core: core);
        await SeedResourcesAsync(core: core);
        await SeedComponentsAsync(core: core);
        await SeedScriptsAsync(core: core);
    }

    private static async Task SeedCommonObjectsAsync(DbContext core)
    {
        if (await core.Set<CommonObject>()
            .AnyAsync())
        {
            return;
        }

        CommonObject[] commonObjects = AcceptanceSeedData
            .LoadCommonObjects()
            .Select(selector: item => new CommonObject
            {
                Name = item.Name,
                Description = item.Description,
                LastUpdated = item.LastUpdated,
                LastUpdatedBy = item.LastUpdatedBy,
                CreatedOn = item.CreatedOn,
                CreatedBy = item.CreatedBy,
                Version = item.Version,
                Key = item.Key,
                Type = item.Type,
                Json = item.Json,
                Culture = item.Culture,
            })
            .ToArray();

        await core.Set<CommonObject>()
            .AddRangeAsync(entities: commonObjects);

        await core.SaveChangesAsync();
    }

    private static void RefreshCaches(IServiceProvider services)
    {
        cCoder.ContentManagement.Exposures.Caching.ICommonObjectCache commonObjectCache =
            services.GetRequiredService<cCoder.ContentManagement.Exposures.Caching.ICommonObjectCache>();

        ContentMetadataCache metadataCache = services.GetRequiredService<ContentMetadataCache>();

        commonObjectCache.Refresh();
        metadataCache.Rebuild();
    }

    private static async Task SeedLayoutsAsync(DbContext core)
    {
        if (await core.Set<Layout>()
            .AnyAsync(predicate: layout => layout.AppId == AppId))
        {
            return;
        }

        Layout[] layouts = AcceptanceSeedData
            .LoadLayoutPackageItems(packageName: "Layouts", itemType: "Core/Layout")
            .Select(selector: layout => new Layout
            {
                AppId = AppId,
                Name = layout.Name,
                Description = layout.Description,
                HeaderHtml = layout.HeaderHtml,
                Html = layout.Html,
                Script = layout.Script,
                CreatedOn = layout.CreatedOn,
                CreatedBy = layout.CreatedBy,
                LastUpdated = layout.LastUpdated,
                LastUpdatedBy = layout.LastUpdatedBy,
            })
            .ToArray();

        await core.Set<Layout>()
            .AddRangeAsync(entities: layouts);

        await core.SaveChangesAsync();
    }

    private static async Task SeedTemplatesAsync(DbContext core)
    {
        if (await core.Set<Template>()
            .AnyAsync(predicate: template => template.AppId == AppId))
        {
            return;
        }

        Template[] templates = AcceptanceSeedData
            .LoadTemplatePackageItems(packageName: "Templates", itemType: "Core/Template")
            .Select(selector: template => new Template
            {
                AppId = AppId,
                Name = template.Name,
                Description = template.Description,
                ResourceKey = template.ResourceKey,
                RawString = template.RawString,
                CreatedOn = template.CreatedOn,
                CreatedBy = template.CreatedBy,
                LastUpdated = template.LastUpdated,
                LastUpdatedBy = template.LastUpdatedBy,
            })
            .ToArray();

        await core.Set<Template>()
            .AddRangeAsync(entities: templates);

        await core.SaveChangesAsync();
    }

    private static async Task SeedResourcesAsync(DbContext core)
    {
        if (await core.Set<Resource>()
            .AnyAsync(predicate: resource => resource.AppId == AppId))
        {
            return;
        }

        Resource[] resources = AcceptanceSeedData
            .LoadResourcePackageItems(packageName: "Resources", itemType: "ContentManagement/Resource")
            .Select(selector: resource => new Resource
            {
                AppId = AppId,
                Name = resource.Name,
                Description = resource.Description,
                Key = resource.Key,
                Culture = resource.Culture ?? string.Empty,
                DisplayName = resource.DisplayName,
                ShortDisplayName = resource.ShortDisplayName,
                CreatedOn = resource.CreatedOn,
                CreatedBy = resource.CreatedBy,
                LastUpdated = resource.LastUpdated,
                LastUpdatedBy = resource.LastUpdatedBy,
            })
            .ToArray();

        await core.Set<Resource>()
            .AddRangeAsync(entities: resources);

        await core.SaveChangesAsync();
    }

    private static async Task SeedComponentsAsync(DbContext core)
    {
        if (await core.Set<Component>()
            .AnyAsync(predicate: component => component.AppId == AppId))
        {
            return;
        }

        Component[] components = AcceptanceSeedData
            .LoadComponentPackageItems(packageName: "Components", itemType: "ContentManagement/Component")
            .Select(selector: component => new Component
            {
                AppId = AppId,
                Name = component.Name,
                Description = component.Description,
                ResourceKey = component.ResourceKey,
                Content = component.Content,
                Script = component.Script,
                Key = component.Key,
                CreatedOn = component.CreatedOn,
                CreatedBy = component.CreatedBy,
                LastUpdated = component.LastUpdated,
                LastUpdatedBy = component.LastUpdatedBy,
            })
            .ToArray();

        await core.Set<Component>()
            .AddRangeAsync(entities: components);

        await core.SaveChangesAsync();
    }

    private static async Task SeedScriptsAsync(DbContext core)
    {
        if (await core.Set<Script>()
            .AnyAsync(predicate: script => script.AppId == AppId))
        {
            return;
        }

        Script[] scripts = AcceptanceSeedData
            .LoadScriptPackageItems(packageName: "Scripts", itemType: "ContentManagement/Script")
            .Select(selector: script => new Script
            {
                AppId = AppId,
                Name = script.Name,
                Description = script.Description,
                Key = script.Key,
                Content = script.Content,
                CreatedOn = script.CreatedOn,
                CreatedBy = script.CreatedBy,
                LastUpdated = script.LastUpdated,
                LastUpdatedBy = script.LastUpdatedBy,
            })
            .ToArray();

        await core.Set<Script>()
            .AddRangeAsync(entities: scripts);

        await core.SaveChangesAsync();
    }
}