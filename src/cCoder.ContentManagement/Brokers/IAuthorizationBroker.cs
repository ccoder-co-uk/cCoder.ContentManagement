// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Brokers;

public interface IAuthorizationBroker
{
    User GetCurrentUser();

    string GetCurrentUserId();

    User GetUserWithRoles(string userId);

    App GetAppWithRoles(int appId);

    Role[] GetRolesForUser(string userId);

    bool HasApps();
}