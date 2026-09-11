// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Models;
using cCoder.Data.Extensions;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;



namespace cCoder.ContentManagement.Services.Foundations.Authorization;

internal partial class AuthorizationService(
    IAuthorizationBroker authorizationBroker) : IAuthorizationService
{
    public void AuthorizeAuthorizationContext(AuthorizationContext authorizationContext) =>
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

    public AuthorizationContext ResolveCurrentAuthorizationContext(
        AuthorizationContext authorizationContext) =>
        TryCatch<AuthorizationContext>(operation: () =>
    {
        ValidateResolveCurrentAuthorizationContext(inputs: [authorizationContext]);
        authorizationContext.User = authorizationBroker.GetCurrentUser();
        authorizationContext.UserId = authorizationBroker.GetCurrentUserId();

        return authorizationContext;
    });

    public bool IsAdminAuthorizationContext(AuthorizationContext authorizationContext) =>
        TryCatch<bool>(operation: () =>
    {
        ValidateIsAdmin(inputs: [authorizationContext]);

        User user = authorizationBroker.GetUserWithRoles(
            userId: authorizationContext.Request.UserName);

        App app = authorizationBroker.GetAppWithRoles(
            appId: authorizationContext.Request.AppId.Value);

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

    public bool UserCanPageAuthorizationContext(
        AuthorizationContext authorizationContext) =>
        TryCatch<bool>(operation: () =>
    {
        ValidateUserCanPageAuthorization(inputs: [authorizationContext]);
        PageAuthorization pageAuthorization = authorizationContext.PageAuthorization;

        Guid[] userRoles = pageAuthorization.User?.Roles?
            .Select(selector: role => role.RoleId)
            .ToArray()
            ?? [];

        return IsAdminOfApp(
            user: pageAuthorization.User,
            appId: pageAuthorization.Page.AppId)
            || (pageAuthorization.Page.Roles?
                .Where(predicate: pageRole =>
                    userRoles.Contains(value: pageRole.RoleId))
                .SelectMany(selector: pageRole =>
                    pageRole.Role?.Privileges ?? [])
                .Contains(
                    value: pageAuthorization.Privilege?
                        .ToLowerInvariant()
                        ?? string.Empty)
                ?? false);
    });

    private bool HasPrivilege(
        string userId,
        int? appId,
        string privilege)
    {
        string normalizedPrivilege = privilege.ToLowerInvariant();
        Role[] userRoles = GetUserRoles(userId: userId);

        return appId.HasValue
            && HasAppAdminPrivilege(userId: userId, appId: appId.Value)
            || userRoles.Any(
                predicate: role =>
                    (!appId.HasValue || role.AppId == appId)
                    && role.Privileges.Any(
                        predicate: foundPrivilege => string.Equals(
                            a: foundPrivilege,
                            b: normalizedPrivilege,
                            comparisonType:
                                StringComparison.OrdinalIgnoreCase)));
    }

    private bool HasAppAdminPrivilege(string userId, int? appId) =>
        GetUserRoles(userId: userId)
            .Any(
                predicate: role =>
                    role.AppId == appId
                    && role.Privileges.Any(
                        predicate: privilege => string.Equals(
                            a: privilege,
                            b: "app_admin",
                            comparisonType:
                                StringComparison.OrdinalIgnoreCase)))
        || !authorizationBroker.HasApps();

    private Role[] GetUserRoles(string userId)
    {
        Role[] userRoles =
            authorizationBroker.GetRolesForUser(userId: userId);

        if (string.Equals(
            a: userId,
            b: "Guest",
            comparisonType: StringComparison.OrdinalIgnoreCase))
        {
            return userRoles;
        }

        Role[] guestRoles =
            authorizationBroker.GetRolesForUser(userId: "Guest");

        return userRoles
            .Concat(second: guestRoles)
            .GroupBy(keySelector: role => role.Id)
            .Select(selector: group => group.First())
            .ToArray();
    }

    private string ResolveCurrentUserId()
    {
        string userId = authorizationBroker.GetCurrentUserId();

        return string.IsNullOrWhiteSpace(value: userId)
            ? "Guest"
            : userId;
    }

    private static bool IsAdminOfApp(User user, int appId) =>
        user?.Roles?.Any(
            predicate: role =>
                role.Role?.AppId == appId
                && (role.Role?.Privileges?.Contains(item: "app_admin")
                    ?? false))
        ?? false;

}