// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Foundations.Authorizations;
using cCoder.ContentManagement.Services.Foundations.HttpContexts;
using cCoder.ContentManagement.Services.Processings;

namespace cCoder.ContentManagement.Services.Orchestrations.PageContexts;

internal sealed partial class PageContextOrchestrationService(
    IHttpContextService httpContextService,
    IPageAuthorizationService pageAuthorizationService,
    IAuthorizationProcessingService authorizationProcessingService)
        : IPageContextOrchestrationService
{
    public ValueTask<HttpPageRenderContext>
        ResolvePageRenderContextAsync() =>
        TryCatch(operation: async () =>
    {

        HttpPageRenderContext context =
            httpContextService.GetPageRenderContext();

        bool hasCultureOverride = context.CultureWasExplicitlyRequested;

        context = await pageAuthorizationService
            .AuthorizeHttpPageRenderContextAsync(
                pageRenderContext: context);

        context.User = authorizationProcessingService
            .ResolveCurrentAuthorizationContext(
                context: new AuthorizationContext
                {
                    Culture = context.Culture
                })
            .User;

        if (!hasCultureOverride
            && !string.IsNullOrWhiteSpace(
                value: context.User?.DefaultCultureId))
        {
            context.Culture = context.User.DefaultCultureId;
        }

        return context;

    }, isValueTask: true);
}