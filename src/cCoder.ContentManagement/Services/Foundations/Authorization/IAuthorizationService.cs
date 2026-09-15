// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Foundations.Authorization;

internal interface IAuthorizationService
{
    AuthorizationData GetAppWithRoles(int appId);

    AuthorizationData GetCurrentUser();

    string GetCurrentUserId();

    AuthorizationData GetRolesForUser(string userId);

    AuthorizationData GetUserWithRoles(string userId);

    bool HasApps();
}