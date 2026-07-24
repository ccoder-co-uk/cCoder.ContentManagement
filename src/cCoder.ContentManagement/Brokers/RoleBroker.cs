// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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

    public async ValueTask<Role> AddRoleAsync(Role newRole)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Role result = (await coreDataContext.Roles.AddAsync(entity: newRole)).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<Role> UpdateRoleAsync(Role updatedRole)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        Role result = coreDataContext.Roles.Update(entity: updatedRole)
            .Entity;

        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteRoleAsync(Role deletedRole)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Roles.Remove(entity: deletedRole);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllRolesAsync(IEnumerable<Role> deletedRole)
    {
        if (deletedRole == null || !deletedRole.Any())
        {
            return;
        }

        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Roles.RemoveRange(entities: deletedRole);
        await coreDataContext.SaveChangesAsync();
    }
}