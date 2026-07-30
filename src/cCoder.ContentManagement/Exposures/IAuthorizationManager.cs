// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Exposures;

public interface IAuthorizationManager
{
    User GetCurrentUser();

    string GetCurrentUserId();

    void Authorize(int? appId, string privilege);

    bool IsAdmin(int appId, string userName);

    bool IsAdminOfApp(int appId);

    bool UserCanPageAuthorization(PageAuthorization pageAuthorization);
}