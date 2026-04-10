using cCoder.Data;
using cCoder.Data.Models.Security;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Brokers;

internal class UserRoleBroker(ICoreContextFactory coreContextFactory) : IUserRoleBroker
{
    public IQueryable<UserRole> GetAllUserRoles(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        IQueryable<UserRole> result;
        if (!ignoreFilters)
        {
            IQueryable<UserRole> userRoles = coreDataContext.UserRoles;
            result = userRoles;
        }
        else
        {
            result = coreDataContext.UserRoles.IgnoreQueryFilters();
        }
        return result;
    }

    public async ValueTask<UserRole> AddUserRoleAsync(UserRole entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        UserRole result = (await coreDataContext.UserRoles.AddAsync(entity)).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteUserRoleAsync(UserRole entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.UserRoles.Remove(entity);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllUserRolesAsync(IEnumerable<UserRole> items)
    {
        if (items == null || !items.Any())
        {
            return;
        }
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.UserRoles.RemoveRange(items);
        await coreDataContext.SaveChangesAsync();
    }
}
