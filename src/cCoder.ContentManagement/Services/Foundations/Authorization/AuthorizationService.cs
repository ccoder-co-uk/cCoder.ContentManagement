// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Foundations.Authorization;

internal partial class AuthorizationService(
    IAuthorizationBroker authorizationBroker) : IAuthorizationService
{
    public User GetCurrentUser() =>
        TryCatch<User>(operation: () =>
            authorizationBroker.GetCurrentUser());

    public bool IsAdminOfApp(int appId) =>
        TryCatch<bool>(operation: () =>
    {
        ValidateIsAdminOfApp(inputs: [appId]);
        return authorizationBroker.IsAdminOfApp(appId: appId);
    });
}