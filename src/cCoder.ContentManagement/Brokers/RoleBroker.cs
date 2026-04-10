using cCoder.Data;
using cCoder.Data.Models.Security;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Brokers;

internal class RoleBroker(ICoreContextFactory coreContextFactory) : IRoleBroker
{
    public IQueryable<Role> GetAllRoles(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        IQueryable<Role> result;
        if (!ignoreFilters)
        {
            IQueryable<Role> roles = coreDataContext.Roles;
            result = roles;
        }
        else
        {
            result = coreDataContext.Roles.IgnoreQueryFilters();
        }
        return result;
    }

    public async ValueTask<Role> AddRoleAsync(Role entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Role result = (await coreDataContext.Roles.AddAsync(entity)).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<Role> UpdateRoleAsync(Role entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Role result = coreDataContext.Roles.Update(entity).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteRoleAsync(Role entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Roles.Remove(entity);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllRolesAsync(IEnumerable<Role> items)
    {
        if (items == null || !items.Any())
        {
            return;
        }
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Roles.RemoveRange(items);
        await coreDataContext.SaveChangesAsync();
    }
}
