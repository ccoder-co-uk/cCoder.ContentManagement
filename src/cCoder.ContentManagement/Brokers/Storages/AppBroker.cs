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
    public IQueryable<App> GetAllApps(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return Extensions.Data.QueryFilterExtensions.Apply(
            query: coreDataContext.Apps,
            ignoreFilters: ignoreFilters);
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