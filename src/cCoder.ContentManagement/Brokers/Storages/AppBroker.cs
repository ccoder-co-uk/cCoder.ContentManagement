using cCoder.Data;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Brokers.Storages;

public class AppBroker(ICoreContextFactory coreContextFactory) : IAppBroker
{
    public IQueryable<App> GetAllApps(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return ignoreFilters
            ? coreDataContext.Apps.IgnoreQueryFilters()
            : coreDataContext.Apps;
    }

    public async ValueTask<App> AddAppAsync(App entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        App result = (await coreDataContext.Apps.AddAsync(entity)).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<App> UpdateAppAsync(App entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        App result = coreDataContext.Apps.Update(entity).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteAppAsync(App entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Apps.Remove(entity);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAppAggregateAsync(App entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        UserRole[] userRolesToDelete =
            [.. entity.Roles?
                .SelectMany(role => role.Users ?? [])
                .GroupBy(userRole => new { userRole.RoleId, userRole.UserId })
                .Select(group => group.First())
                ?? []];

        if (userRolesToDelete.Length > 0)
        {
            coreDataContext.UserRoles.RemoveRange(userRolesToDelete);
        }

        Role[] rolesToDelete = [.. entity.Roles ?? []];

        if (rolesToDelete.Length > 0)
        {
            coreDataContext.Roles.RemoveRange(rolesToDelete);
        }

        coreDataContext.Apps.Remove(entity);
        await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllAppsAsync(IEnumerable<App> items)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Apps.RemoveRange(items);
        await coreDataContext.SaveChangesAsync();
    }
}
