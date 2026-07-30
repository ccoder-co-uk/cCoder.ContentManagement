// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Exposures;

internal sealed class AuthorizationManager(
    IAuthorizationProcessingService authorizationProcessingService)
        : IAuthorizationManager
{
    public User GetCurrentUser() =>
        authorizationProcessingService.ResolveCurrentAuthorizationContext(
            context: new AuthorizationContext()).User;

    public string GetCurrentUserId() =>
        authorizationProcessingService.ResolveCurrentAuthorizationContext(
            context: new AuthorizationContext()).UserId;

    public void Authorize(int? appId, string privilege) =>
        authorizationProcessingService.AuthorizeAuthorizationContext(
            context: new AuthorizationContext
            {
                Request = new AuthorizationRequest
                {
                    AppId = appId,
                    Privilege = privilege
                }
            });

    public bool IsAdmin(int appId, string userName) =>
        authorizationProcessingService.IsAdminAuthorizationContext(
            context: new AuthorizationContext
            {
                Request = new AuthorizationRequest
                {
                    AppId = appId,
                    UserName = userName
                }
            });

    public bool IsAdminOfApp(int appId) =>
        authorizationProcessingService.IsAdminOfAppAuthorizationContext(
            context: new AuthorizationContext
            {
                AppId = appId
            });

    public bool UserCanPageAuthorization(
        PageAuthorization pageAuthorization) =>
        authorizationProcessingService.UserCanPageAuthorizationContext(
            context: new AuthorizationContext
            {
                PageAuthorization = pageAuthorization
            });
}