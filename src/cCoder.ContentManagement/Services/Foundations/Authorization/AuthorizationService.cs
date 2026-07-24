// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Foundations.Authorization;

internal partial class AuthorizationService(
    IAuthorizationBroker authorizationBroker) : IAuthorizationService
{
    public void Authorize(int? appId, string privilege) =>
        TryCatch(operation: () =>
    {
        ValidateAuthorize(inputs: [appId, privilege]);
        authorizationBroker.Authorize(appId: appId, privilege: privilege);
    });

    public User GetCurrentUser() =>
        TryCatch<User>(operation: () =>
            authorizationBroker.GetCurrentUser());

    public bool IsAdmin(int appId, string userName) =>
        TryCatch<bool>(operation: () =>
    {
        ValidateIsAdmin(inputs: [appId, userName]);
        return authorizationBroker.IsAdmin(appId: appId, userName: userName);
    });

    public bool IsAdminOfApp(int appId) =>
        TryCatch<bool>(operation: () =>
    {
        ValidateIsAdminOfApp(inputs: [appId]);
        return authorizationBroker.IsAdminOfApp(appId: appId);
    });
}