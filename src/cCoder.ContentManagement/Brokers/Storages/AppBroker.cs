// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Brokers.Storages;

internal sealed class AppBroker(ICoreContextFactory coreContextFactory) : IAppBroker
{
    public IQueryable<App> GetAllApps()
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.Apps;
    }

    public IQueryable<App> GetAllAppsIgnoringFilters()
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.Apps.IgnoreQueryFilters();
    }

    public async ValueTask<App> GetAppForRenderAsync(int appId)
    {
        await using CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        return await coreDataContext.Apps
            .IgnoreQueryFilters()
            .Include(navigationPropertyPath: app => app.Cultures)
            .Include(navigationPropertyPath: app => app.Layouts)
            .Include(navigationPropertyPath: app => app.Templates)
            .Include(navigationPropertyPath: app => app.Resources)
            .Include(navigationPropertyPath: app => app.Components)
            .Include(navigationPropertyPath: app => app.Scripts)
            .Include(navigationPropertyPath: app => app.Pages)
                .ThenInclude(navigationPropertyPath: page => page.PageInfo)
            .SingleOrDefaultAsync(predicate: app => app.Id == appId);
    }

    public App GetAppForDelete(int appId)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.Apps
            .IgnoreQueryFilters()
            .Include(navigationPropertyPath: app => app.Roles)
            .ThenInclude(navigationPropertyPath: role => role.Users)
            .FirstOrDefault(predicate: app => app.Id == appId);
    }

    public Culture[] GetCultures()
    {
        using CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        return [.. coreDataContext.Cultures.IgnoreQueryFilters()];
    }

    public Privilege[] GetPrivileges()
    {
        using CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        return [.. coreDataContext.Privileges.IgnoreQueryFilters()];
    }

    public User GetCurrentUser()
    {
        using CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        return coreDataContext.User;
    }

    public string GetCurrentUserId()
    {
        using CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        return coreDataContext.AuthInfo?.SSOUserId;
    }

    public async ValueTask<App> AddAppAsync(App newApp)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        App result = (await coreDataContext.Apps.AddAsync(entity: newApp)).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<App> UpdateAppAsync(App updatedApp)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        App result = coreDataContext.Apps.Update(entity: updatedApp)
            .Entity;

        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask PersistNewAppRolesAsync(App app)
    {
        await using CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        Role[] storageRoles = [.. (app.Roles ?? [])
            .Select(selector: role => new Role
            {
                Id = role.Id,
                AppId = app.Id,
                Name = role.Name,
                Description = role.Description,
                Privs = role.Privs
            })];

        UserRole[] storageUserRoles = [.. (app.Roles ?? [])
            .SelectMany(selector: role => (role.Users ?? [])
                .Select(selector: userRole => new UserRole
                {
                    RoleId = role.Id,
                    UserId = userRole.UserId
                }))];

        await coreDataContext.Roles.AddRangeAsync(entities: storageRoles);

        await coreDataContext.UserRoles.AddRangeAsync(
            entities: storageUserRoles);

        await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask<int> DeleteAppAsync(App deletedApp)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Apps.Remove(entity: deletedApp);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAppAggregateAsync(App deletedApp)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        UserRole[] userRolesToDelete =
            [.. deletedApp.Roles?
                .SelectMany(selector: role => role.Users ?? [])
            .GroupBy(keySelector: userRole => new { userRole.RoleId, userRole.UserId })
            .Select(selector: group => group.First())
                ?? []];

        coreDataContext.UserRoles.RemoveRange(entities: userRolesToDelete);

        Role[] rolesToDelete = [.. deletedApp.Roles ?? []];
        Guid[] roleIds = [.. rolesToDelete.Select(selector: role => role.Id)];

        FolderRole[] folderRolesToDelete =
            [.. coreDataContext.FolderRoles
                .IgnoreQueryFilters()
                .Where(predicate: folderRole =>
                    roleIds.Contains(value: folderRole.RoleId))];

        PageRole[] pageRolesToDelete =
            [.. coreDataContext.PageRoles
                .IgnoreQueryFilters()
                .Where(predicate: pageRole =>
                    roleIds.Contains(value: pageRole.RoleId))];

        coreDataContext.FolderRoles.RemoveRange(
            entities: folderRolesToDelete);

        coreDataContext.PageRoles.RemoveRange(
            entities: pageRolesToDelete);

        coreDataContext.Roles.RemoveRange(entities: rolesToDelete);

        coreDataContext.Apps.Remove(entity: deletedApp);
        await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllAppsAsync(IEnumerable<App> deletedApp)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Apps.RemoveRange(entities: deletedApp);
        await coreDataContext.SaveChangesAsync();
    }
}