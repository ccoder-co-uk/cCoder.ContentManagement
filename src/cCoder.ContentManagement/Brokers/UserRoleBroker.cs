// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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

    public async ValueTask<UserRole> AddUserRoleAsync(UserRole newUserRole)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        UserRole result = (await coreDataContext.UserRoles.AddAsync(entity: newUserRole)).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteUserRoleAsync(UserRole deletedUserRole)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.UserRoles.Remove(entity: deletedUserRole);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllUserRolesAsync(IEnumerable<UserRole> deletedUserRole)
    {
        if (deletedUserRole == null || !deletedUserRole.Any())
        {
            return;
        }

        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.UserRoles.RemoveRange(entities: deletedUserRole);
        await coreDataContext.SaveChangesAsync();
    }
}