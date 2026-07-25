// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Data;
using cCoder.Data.Extensions;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Dependencies;

internal static class AuthorizationDependency
{
    internal static bool IsAdminOfApp(
        CoreDataContext coreDataContext,
        int? appId)
    {
        if (!appId.HasValue)
        {
            return false;
        }

        string currentUserId = GetCurrentUserId(coreDataContext: coreDataContext);

        return HasAppAdminPrivilege(
            coreDataContext: coreDataContext,
            userId: currentUserId,
            appId: appId.Value);
    }

    internal static bool IsAdmin(
        CoreDataContext coreDataContext,
        int appId,
        string userName)
    {
        User user = coreDataContext.Users
            .Include(navigationPropertyPath: foundUser => foundUser.Roles)
            .FirstOrDefault(predicate: foundUser => foundUser.Id == userName);

        return coreDataContext.Apps
            .Include(navigationPropertyPath: foundApp => foundApp.Roles.Select(selector: role => role.Users))
            .FirstOrDefault(predicate: foundApp => foundApp.Id == appId)?
            .IsAppAdmin(user: user) ?? false;
    }

    internal static void Authorize(
        CoreDataContext coreDataContext,
        int? appId,
        string privilege)
    {
        string currentUserId = GetCurrentUserId(coreDataContext: coreDataContext);

        if (!HasAppAdminPrivilege(
            coreDataContext: coreDataContext,
            userId: currentUserId,
            appId: appId) &&
            !HasPrivilege(
                coreDataContext: coreDataContext,
                userId: currentUserId,
                appId: appId,
                privilege: privilege))
        {
            throw new SecurityException(message: "Access Denied!");
        }
    }

    private static string GetCurrentUserId(CoreDataContext coreDataContext)
    {
        string userId = coreDataContext.AuthInfo?.SSOUserId;

        return string.IsNullOrWhiteSpace(value: userId)
            ? "Guest"
            : userId;
    }

    private static bool HasPrivilege(
        CoreDataContext coreDataContext,
        string userId,
        int? appId,
        string privilege)
    {
        string normalizedPrivilege = privilege.ToLowerInvariant();
        Role[] userRoles = GetUserRoles(coreDataContext: coreDataContext, userId: userId);

        return appId.HasValue &&
            HasAppAdminPrivilege(
                coreDataContext: coreDataContext,
                userId: userId,
                appId: appId.Value) ||
            userRoles.Any(
                predicate: role =>
                    (!appId.HasValue || role.AppId == appId) &&
                    role.Privileges.Any(
                        predicate: foundPrivilege => string.Equals(
                            a: foundPrivilege,
                            b: normalizedPrivilege,
                            comparisonType: StringComparison.OrdinalIgnoreCase)));
    }

    private static bool HasAppAdminPrivilege(
        CoreDataContext coreDataContext,
        string userId,
        int? appId) =>
        GetUserRoles(coreDataContext: coreDataContext, userId: userId)
            .Any(
                predicate: role =>
                    role.AppId == appId &&
                    role.Privileges.Any(
                        predicate: privilege => string.Equals(
                            a: privilege,
                            b: "app_admin",
                            comparisonType: StringComparison.OrdinalIgnoreCase))) ||
        !coreDataContext.Apps
            .IgnoreQueryFilters()
            .Any();

    private static Role[] GetUserRoles(
        CoreDataContext coreDataContext,
        string userId)
    {
        Role[] userRoles = LoadRolesForUser(
            coreDataContext: coreDataContext,
            userId: userId);

        if (string.Equals(
            a: userId,
            b: "Guest",
            comparisonType: StringComparison.OrdinalIgnoreCase))
        {
            return userRoles;
        }

        Role[] guestRoles = LoadRolesForUser(
            coreDataContext: coreDataContext,
            userId: "Guest");

        return userRoles
            .Concat(second: guestRoles)
            .GroupBy(keySelector: role => role.Id)
            .Select(selector: group => group.First())
            .ToArray();
    }

    private static Role[] LoadRolesForUser(
        CoreDataContext coreDataContext,
        string userId)
    {
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
}