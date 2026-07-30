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
        AuthorizationContext context) =>
        TryCatch(operation: () =>
    {
        ValidateAuthorize(inputs: [context]);

        authorizationService.AuthorizeAuthorizationContext(
            context: context);
    });

    public AuthorizationContext ResolveCurrentAuthorizationContext(
        AuthorizationContext context) =>
        TryCatch<AuthorizationContext>(operation: () =>
    {
        ValidateResolveCurrentAuthorizationContext(inputs: [context]);

        return authorizationService.ResolveCurrentAuthorizationContext(
            context: context);
    });

    public bool IsAdminAuthorizationContext(
        AuthorizationContext context) =>
        TryCatch<bool>(operation: () =>
    {
        ValidateIsAdmin(inputs: [context]);

        return authorizationService.IsAdminAuthorizationContext(
            context: context);
    });

    public bool IsAdminOfAppAuthorizationContext(
        AuthorizationContext context) =>
        TryCatch<bool>(operation: () =>
    {
        ValidateIsAdminOfApp(inputs: [context]);

        return authorizationService.IsAdminOfAppAuthorizationContext(
            context: context);
    });

    public AuthorizationContext ResolveRenderAuthorizationContext(
        AuthorizationContext context) =>
        TryCatch<AuthorizationContext>(operation: () =>
    {
        ValidateResolveRenderAuthorization(inputs: [context]);

        AuthorizationContext currentContext =
            authorizationService.ResolveCurrentAuthorizationContext(
                context: context);

        currentContext.RenderAuthorization = new()
        {
            Culture = currentContext.Culture
                ?? currentContext.User.DefaultCultureId,
            User = currentContext.User
        };

        return currentContext;
    });

    public bool UserCanPageAuthorizationContext(
        AuthorizationContext context) =>
        TryCatch<bool>(operation: () =>
    {
        ValidateUserCanPageAuthorization(inputs: [context]);

        return authorizationService.UserCanPageAuthorizationContext(
            context: context);
    });
}