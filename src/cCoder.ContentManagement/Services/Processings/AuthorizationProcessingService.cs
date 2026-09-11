// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Foundations.Authorization;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class AuthorizationProcessingService(
    IAuthorizationService authorizationService)
        : IAuthorizationProcessingService
{
    public void AuthorizeAuthorizationContext(
        AuthorizationContext authorizationContext) =>
        TryCatch(operation: () =>
    {
        ValidateAuthorize(inputs: [authorizationContext]);

        authorizationService.AuthorizeAuthorizationContext(
            context: authorizationContext);
    });

    public AuthorizationContext ResolveCurrentAuthorizationContext(
        AuthorizationContext authorizationContext) =>
        TryCatch<AuthorizationContext>(operation: () =>
    {
        ValidateResolveCurrentAuthorizationContext(inputs: [authorizationContext]);

        return authorizationService.ResolveCurrentAuthorizationContext(
            context: authorizationContext);
    });

    public bool IsAdminAuthorizationContext(
        AuthorizationContext authorizationContext) =>
        TryCatch<bool>(operation: () =>
    {
        ValidateIsAdmin(inputs: [authorizationContext]);

        return authorizationService.IsAdminAuthorizationContext(
            context: authorizationContext);
    });

    public bool IsAdminOfAppAuthorizationContext(
        AuthorizationContext authorizationContext) =>
        TryCatch<bool>(operation: () =>
    {
        ValidateIsAdminOfApp(inputs: [authorizationContext]);

        return authorizationService.IsAdminOfAppAuthorizationContext(
            context: authorizationContext);
    });

    public AuthorizationContext ResolveRenderAuthorizationContext(
        AuthorizationContext authorizationContext) =>
        TryCatch<AuthorizationContext>(operation: () =>
    {
        ValidateResolveRenderAuthorization(inputs: [authorizationContext]);

        AuthorizationContext currentContext =
            authorizationService.ResolveCurrentAuthorizationContext(
                context: authorizationContext);

        currentContext.RenderAuthorization = new()
        {
            Culture = currentContext.Culture
                ?? currentContext.User.DefaultCultureId,
            User = currentContext.User
        };

        return currentContext;
    });

    public bool UserCanPageAuthorizationContext(
        AuthorizationContext authorizationContext) =>
        TryCatch<bool>(operation: () =>
    {
        ValidateUserCanPageAuthorization(inputs: [authorizationContext]);

        return authorizationService.UserCanPageAuthorizationContext(
            context: authorizationContext);
    });
}