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
    public void AuthorizeAuthorizationContext(AuthorizationContext context) =>
        TryCatch(operation: () =>
    {
        ValidateAuthorize(inputs: [context]);
        string userId = ResolveCurrentUserId();

        if (!HasAppAdminPrivilege(
            userId: userId,
            appId: context.Request.AppId)
            && !HasPrivilege(
                userId: userId,
                appId: context.Request.AppId,
                privilege: context.Request.Privilege))
        {
            throw new SecurityException(message: "Access Denied!");
        }
    });

    public AuthorizationContext ResolveCurrentAuthorizationContext(
        AuthorizationContext context) =>
        TryCatch<AuthorizationContext>(operation: () =>
    {
        ValidateResolveCurrentAuthorizationContext(inputs: [context]);
        context.User = authorizationBroker.GetCurrentUser();
        context.UserId = authorizationBroker.GetCurrentUserId();

        return context;
    });

    public bool IsAdminAuthorizationContext(AuthorizationContext context) =>
        TryCatch<bool>(operation: () =>
    {
        ValidateIsAdmin(inputs: [context]);

        User user = authorizationBroker.GetUserWithRoles(
            userId: context.Request.UserName);

        App app = authorizationBroker.GetAppWithRoles(
            appId: context.Request.AppId.Value);

        return app?.IsAppAdmin(user: user) ?? false;
    });

    public bool IsAdminOfAppAuthorizationContext(
        AuthorizationContext context) =>
        TryCatch<bool>(operation: () =>
    {
        ValidateIsAdminOfApp(inputs: [context]);

        return HasAppAdminPrivilege(
            userId: ResolveCurrentUserId(),
            appId: context.AppId);
    });

    public bool UserCanPageAuthorizationContext(
        AuthorizationContext context) =>
        TryCatch<bool>(operation: () =>
    {
        ValidateUserCanPageAuthorization(inputs: [context]);
        PageAuthorization pageAuthorization = context.PageAuthorization;

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