// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Foundations.Authorization;

internal partial class AuthorizationService(
    IAuthorizationBroker authorizationBroker) : IAuthorizationService
{
    public AuthorizationData GetAppWithRoles(int appId) =>
        TryCatch(operation: () =>
    {
        ValidateAppWithRolesOnGet(inputs: [appId]);

        return new AuthorizationData
        {
            App = authorizationBroker.GetAppWithRoles(appId: appId)
        };
    });

    public AuthorizationData GetCurrentUser() =>
        TryCatch(operation: () =>
    {
        return new AuthorizationData
        {
            User = authorizationBroker.GetCurrentUser()
        };
    });

    public string GetCurrentUserId() =>
        TryCatch(operation: () =>
    {
        return authorizationBroker.GetCurrentUserId();
    });

    public AuthorizationData GetRolesForUser(string userId) =>
        TryCatch(operation: () =>
    {
        ValidateRolesForUserOnGet(inputs: [userId]);

        return new AuthorizationData
        {
            Roles = authorizationBroker.GetRolesForUser(userId: userId)
        };
    });

    public AuthorizationData GetUserWithRoles(string userId) =>
        TryCatch(operation: () =>
    {
        ValidateUserWithRolesOnGet(inputs: [userId]);

        return new AuthorizationData
        {
            User = authorizationBroker.GetUserWithRoles(userId: userId)
        };
    });

    public bool HasApps() =>
        TryCatch(operation: () =>
    {
        return authorizationBroker.HasApps();
    });
}