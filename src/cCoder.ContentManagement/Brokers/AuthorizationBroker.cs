// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Brokers;

internal class AuthorizationBroker(ICoreContextFactory coreContextFactory) : IAuthorizationBroker
{
    public User GetCurrentUser()
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        return coreDataContext.User;
    }

    public string GetCurrentUserId()
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.AuthInfo?.SSOUserId;
    }

    public bool IsAdminOfApp(int? appId)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return Extensions.Data.AuthorizationExtensions.IsAdminOfApp(
            coreDataContext: coreDataContext,
            appId: appId);
    }

    public bool IsAdmin(int appId, string userName)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return Extensions.Data.AuthorizationExtensions.IsAdmin(
            coreDataContext: coreDataContext,
            appId: appId,
            userName: userName);
    }

    public void Authorize(int? appId, string privilege)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        Extensions.Data.AuthorizationExtensions.Authorize(
            coreDataContext: coreDataContext,
            appId: appId,
            privilege: privilege);
    }
}