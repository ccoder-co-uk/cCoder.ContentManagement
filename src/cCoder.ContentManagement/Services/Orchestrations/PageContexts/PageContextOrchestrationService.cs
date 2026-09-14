// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Processings;
using cCoder.ContentManagement.Services.Processings.HttpContexts;
using cCoder.ContentManagement.Services.Processings.PageContexts;

namespace cCoder.ContentManagement.Services.Orchestrations.PageContexts;

internal sealed partial class PageContextOrchestrationService(
    IHttpContextProcessingService httpContextProcessingService,
    IPageAuthorizationProcessingService pageAuthorizationProcessingService,
    IAuthorizationProcessingService authorizationProcessingService)
        : IPageContextOrchestrationService
{
    public ValueTask<HttpPageRenderContext>
        ResolvePageRenderContextAsync() =>
        TryCatch(operation: async () =>
    {

        HttpPageRenderContext context =
            httpContextProcessingService.GetPageRenderContext();

        bool hasCultureOverride = context.CultureWasExplicitlyRequested;

        context = await pageAuthorizationProcessingService
            .AuthorizeHttpPageRenderContextAsync(
                httpPageRenderContext: context);

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