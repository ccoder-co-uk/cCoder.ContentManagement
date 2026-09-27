// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Foundations.Authorization;
using cCoder.Data.Extensions;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class AuthorizationProcessingService(
    IAuthorizationService authorizationService)
        : IAuthorizationProcessingService
{
    public void AuthorizeAuthorizationContext(
        AuthorizationContext authorizationContext) =>
        TryCatch(operation: () =>
    {
        ValidateAuthorize(inputs: [authorizationContext]);
        string userId = ResolveCurrentUserId();

        if (!HasAppAdminPrivilege(
            userId: userId,
            appId: authorizationContext.Request.AppId)
            && !HasPrivilege(
                userId: userId,
                appId: authorizationContext.Request.AppId,
                privilege: authorizationContext.Request.Privilege))
        {
            throw new SecurityException(message: "Access Denied!");
        }
    });

    public string GetCurrentUserId() =>
        TryCatch<string>(operation: () =>
    {
        return authorizationService.GetCurrentUserId();
    });

    public AuthorizationContext ResolveCurrentAuthorizationContext(
        AuthorizationContext authorizationContext) =>
        TryCatch<AuthorizationContext>(operation: () =>
    {
        ValidateResolveCurrentAuthorizationContext(inputs: [authorizationContext]);
        authorizationContext.User = authorizationService.GetCurrentUser().User;
        authorizationContext.UserId = authorizationService.GetCurrentUserId();

        return authorizationContext;
    });

    public bool IsAdminAuthorizationContext(
        AuthorizationContext authorizationContext) =>
        TryCatch<bool>(operation: () =>
    {
        ValidateIsAdmin(inputs: [authorizationContext]);

        User user = authorizationService.GetUserWithRoles(
            userId: authorizationContext.Request.UserName).User;

        App app = authorizationService.GetAppWithRoles(
            appId: authorizationContext.Request.AppId.Value).App;

        return app?.IsAppAdmin(user: user) ?? false;
    });

    public bool IsAdminOfAppAuthorizationContext(
        AuthorizationContext authorizationContext) =>
        TryCatch<bool>(operation: () =>
    {
        ValidateIsAdminOfApp(inputs: [authorizationContext]);

        return HasAppAdminPrivilege(
            userId: ResolveCurrentUserId(),
            appId: authorizationContext.AppId);
    });

    public AuthorizationContext ResolveRenderAuthorizationContext(
        AuthorizationContext authorizationContext) =>
        TryCatch<AuthorizationContext>(operation: () =>
    {
        ValidateResolveRenderAuthorization(inputs: [authorizationContext]);

        AuthorizationContext currentContext =
            ResolveCurrentAuthorizationContextInternal(
                authorizationContext: authorizationContext);

        currentContext.RenderAuthorization = new()
        {
            Culture = currentContext.Culture
                ?? currentContext.User.DefaultCultureId,
            User = currentContext.User
        };

        return currentContext;
    });

    public bool UserCanPageAuthorizationContext(
        AuthorizationContext authorizationContext) =>
        TryCatch<bool>(operation: () =>
    {
        ValidateUserCanPageAuthorization(inputs: [authorizationContext]);
        PageAuthorization pageAuthorization = authorizationContext.PageAuthorization;

        if (IsAdminOfApp(
            user: pageAuthorization.User,
            appId: pageAuthorization.Page.AppId))
        {
            return true;
        }

        string requestedPrivilege = pageAuthorization.Privilege?
            .ToLowerInvariant()
            ?? string.Empty;

        foreach (PageRole pageRole in pageAuthorization.Page.Roles ?? [])
        {
            bool userIsInRole = false;

            foreach (UserRole userRole in pageAuthorization.User?.Roles ?? [])
            {
                if (userRole.RoleId == pageRole.RoleId)
                {
                    userIsInRole = true;
                    break;
                }
            }

            if (!userIsInRole)
            {
                continue;
            }

            foreach (string privilege in pageRole.Role?.Privileges ?? [])
            {
                if (string.Equals(
                    a: privilege,
                    b: requestedPrivilege,
                    comparisonType: StringComparison.Ordinal))
                {
                    return true;
                }
            }
        }

        return false;
    });

    private bool HasPrivilege(
        string userId,
        int? appId,
        string privilege)
    {
        string normalizedPrivilege = privilege.ToLowerInvariant();
        Role[] userRoles = GetUserRoles(userId: userId);

        if (appId.HasValue
            && HasAppAdminPrivilege(userId: userId, appId: appId.Value))
        {
            return true;
        }

        foreach (Role role in userRoles)
        {
            if (appId.HasValue && role.AppId != appId)
            {
                continue;
            }

            foreach (string foundPrivilege in role.Privileges)
            {
                if (string.Equals(
                    a: foundPrivilege,
                    b: normalizedPrivilege,
                    comparisonType: StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool HasAppAdminPrivilege(string userId, int? appId)
    {
        foreach (Role role in GetUserRoles(userId: userId))
        {
            if (role.AppId != appId)
            {
                continue;
            }

            foreach (string privilege in role.Privileges)
            {
                if (string.Equals(
                    a: privilege,
                    b: "app_admin",
                    comparisonType: StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
        }

        return !authorizationService.HasApps();
    }

    private Role[] GetUserRoles(string userId)
    {
        Role[] userRoles =
            authorizationService.GetRolesForUser(userId: userId).Roles;

        if (string.Equals(
            a: userId,
            b: "Guest",
            comparisonType: StringComparison.OrdinalIgnoreCase))
        {
            return userRoles;
        }

        Role[] guestRoles =
            authorizationService.GetRolesForUser(userId: "Guest").Roles;

        List<Role> combinedRoles = [];

        AddUniqueRoles(source: userRoles, destination: combinedRoles);
        AddUniqueRoles(source: guestRoles, destination: combinedRoles);

        return [.. combinedRoles];
    }

    private string ResolveCurrentUserId()
    {
        string userId = authorizationService.GetCurrentUserId();

        return string.IsNullOrWhiteSpace(value: userId)
            ? "Guest"
            : userId;
    }

    private AuthorizationContext ResolveCurrentAuthorizationContextInternal(
        AuthorizationContext authorizationContext)
    {
        authorizationContext.User = authorizationService.GetCurrentUser().User;
        authorizationContext.UserId = authorizationService.GetCurrentUserId();

        return authorizationContext;
    }

    private static void AddUniqueRoles(
        IEnumerable<Role> source,
        ICollection<Role> destination)
    {
        foreach (Role role in source)
        {
            bool roleExists = false;

            foreach (Role existingRole in destination)
            {
                if (existingRole.Id == role.Id)
                {
                    roleExists = true;
                    break;
                }
            }

            if (!roleExists)
            {
                destination.Add(item: role);
            }
        }
    }

    private static bool IsAdminOfApp(User user, int appId)
    {
        foreach (UserRole userRole in user?.Roles ?? [])
        {
            if (userRole.Role?.AppId != appId)
            {
                continue;
            }

            foreach (string privilege in userRole.Role?.Privileges ?? [])
            {
                if (string.Equals(
                    a: privilege,
                    b: "app_admin",
                    comparisonType: StringComparison.Ordinal))
                {
                    return true;
                }
            }
        }

        return false;
    }
}