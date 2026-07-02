using System.Security;
using cCoder.Data;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Brokers;

internal class AuthorizationBroker(ICoreContextFactory coreContextFactory) : IAuthorizationBroker
{
    public User GetCurrentUser()
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        return coreDataContext.User;
    }

    public bool IsAdminOfApp(int? appId)
    {
        if (!appId.HasValue)
            return false;

        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        string currentUserId = GetCurrentUserId(coreDataContext);
        return HasAppAdminPrivilege(coreDataContext, currentUserId, appId.Value);
    }

    public bool IsAdmin(int appId, string userName)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        User user = coreDataContext.Users
            .Include(foundUser => foundUser.Roles)
            .FirstOrDefault(foundUser => foundUser.Id == userName);

        return coreDataContext.Apps
            .Include(foundApp => foundApp.Roles.Select(role => role.Users))
            .FirstOrDefault(foundApp => foundApp.Id == appId)?.IsAppAdmin(user) ?? false;
    }

    public void Authorize(int? appId, string privilege)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        string currentUserId = GetCurrentUserId(coreDataContext);
        if (!HasAppAdminPrivilege(coreDataContext, currentUserId, appId) && !HasPrivilege(coreDataContext, currentUserId, appId, privilege))
            throw new SecurityException("Access Denied!");
    }

    private static string GetCurrentUserId(CoreDataContext coreDataContext)
    {
        string text = coreDataContext.AuthInfo?.SSOUserId;
        return string.IsNullOrWhiteSpace(text) ? "Guest" : text;
    }

    private static bool HasPrivilege(CoreDataContext coreDataContext, string userId, int? appId, string privilege)
    {
        string normalizedPrivilege = privilege.ToLowerInvariant();
        Role[] userRoles = GetUserRoles(coreDataContext, userId);
        return (appId.HasValue && HasAppAdminPrivilege(coreDataContext, userId, appId.Value)) || userRoles.Any(role =>
            (!appId.HasValue || role.AppId == appId)
            && role.Privileges.Any(foundPrivilege => string.Equals(foundPrivilege, normalizedPrivilege, StringComparison.OrdinalIgnoreCase)));
    }

    private static bool HasAppAdminPrivilege(CoreDataContext coreDataContext, string userId, int? appId)
    {
        return GetUserRoles(coreDataContext, userId)
            .Any(role => role.AppId == appId && role.Privileges.Any(privilege => string.Equals(privilege, "app_admin", StringComparison.OrdinalIgnoreCase))) ||
                !coreDataContext.Roles.IgnoreQueryFilters().Any();
    }

    private static Role[] GetUserRoles(CoreDataContext coreDataContext, string userId)
    {
        Role[] array = LoadRolesForUser(coreDataContext, userId);
        if (string.Equals(userId, "Guest", StringComparison.OrdinalIgnoreCase))
            return array;

        Role[] second = LoadRolesForUser(coreDataContext, "Guest");
        return array.Concat(second)
            .GroupBy(role => role.Id)
            .Select(group => group.First())
            .ToArray();
    }

    private static Role[] LoadRolesForUser(CoreDataContext coreDataContext, string userId)
    {
        Guid[] roleIds = coreDataContext.UserRoles
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(userRole => userRole.UserId == userId)
            .Select(userRole => userRole.RoleId)
            .Distinct()
            .ToArray();

        return coreDataContext.Roles
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(role => roleIds.Contains(role.Id))
            .ToArray();
    }
}
