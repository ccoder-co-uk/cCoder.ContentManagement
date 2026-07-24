// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Foundations.Authorization;

public interface IAuthorizationService
{
    User GetCurrentUser();

    bool IsAdminOfApp(int appId);
}