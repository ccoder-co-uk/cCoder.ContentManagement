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