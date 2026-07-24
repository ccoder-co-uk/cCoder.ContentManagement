// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Foundations.Authorization;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class AuthorizationProcessingService(
    IAuthorizationService authorizationService)
        : IAuthorizationProcessingService
{
    public void Authorize(int? appId, string privilege) =>
        TryCatch(operation: () =>
    {
        ValidateAuthorize(inputs: [appId, privilege]);
        authorizationService.Authorize(appId: appId, privilege: privilege);
    });

    public bool IsAdmin(int appId, string userName) =>
        TryCatch<bool>(operation: () =>
    {
        ValidateIsAdmin(inputs: [appId, userName]);
        return authorizationService.IsAdmin(appId: appId, userName: userName);
    });

    public bool IsAdminOfApp(int appId) =>
        TryCatch<bool>(operation: () =>
    {
        ValidateIsAdminOfApp(inputs: [appId]);
        return authorizationService.IsAdminOfApp(appId: appId);
    });

    public RenderAuthorization ResolveRenderAuthorization(string culture) =>
        TryCatch<RenderAuthorization>(operation: () =>
    {
        ValidateResolveRenderAuthorization(inputs: [culture]);
        User user = authorizationService.GetCurrentUser();

        return new RenderAuthorization
        {
            Culture = culture ?? user.DefaultCultureId,
            User = user
        };

    });
}