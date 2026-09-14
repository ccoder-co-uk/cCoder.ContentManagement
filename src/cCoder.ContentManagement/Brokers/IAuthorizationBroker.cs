// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.CodeAnalysis.Exposures;

namespace cCoder.ContentManagement.Brokers;

public interface IAuthorizationBroker : IUtilityBroker
{
    User GetCurrentUser();

    string GetCurrentUserId();

    User GetUserWithRoles(string userId);

    App GetAppWithRoles(int appId);

    Role[] GetRolesForUser(string userId);

    bool HasApps();
}