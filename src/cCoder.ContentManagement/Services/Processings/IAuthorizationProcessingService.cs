// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

internal interface IAuthorizationProcessingService
{
    void AuthorizeAuthorizationContext(AuthorizationContext context);

    AuthorizationContext ResolveCurrentAuthorizationContext(
        AuthorizationContext context);

    bool IsAdminAuthorizationContext(AuthorizationContext context);

    bool IsAdminOfAppAuthorizationContext(AuthorizationContext context);

    AuthorizationContext ResolveRenderAuthorizationContext(
        AuthorizationContext context);

    bool UserCanPageAuthorizationContext(AuthorizationContext context);
}