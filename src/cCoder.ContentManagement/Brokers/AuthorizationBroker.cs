// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Brokers;

internal class AuthorizationBroker(
    ICoreContextFactory coreContextFactory) : IAuthorizationBroker
{
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

    public User GetUserWithRoles(string userId)
    {
        using CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        return coreDataContext.Users
            .Include(navigationPropertyPath: user => user.Roles)
            .FirstOrDefault(predicate: user => user.Id == userId);
    }

    public App GetAppWithRoles(int appId)
    {
        using CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        return coreDataContext.Apps
            .Include(
                navigationPropertyPath: app =>
                    app.Roles.Select(selector: role => role.Users))
            .FirstOrDefault(predicate: app => app.Id == appId);
    }

    public Role[] GetRolesForUser(string userId)
    {
        using CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        Guid[] roleIds = coreDataContext.UserRoles
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(predicate: userRole => userRole.UserId == userId)
            .Select(selector: userRole => userRole.RoleId)
            .Distinct()
            .ToArray();

        return coreDataContext.Roles
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(predicate: role => roleIds.Contains(value: role.Id))
            .ToArray();
    }

    public bool HasApps()
    {
        using CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        return coreDataContext.Apps
            .IgnoreQueryFilters()
            .Any();
    }
}