// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Foundations.Authorization;

public interface IAuthorizationService
{
    User GetCurrentUser();

    void Authorize(int? appId, string privilege);

    bool IsAdmin(int appId, string userName);

    bool IsAdminOfApp(int appId);
}