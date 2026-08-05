// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Foundations.Authorizations;
using cCoder.ContentManagement.Services.Foundations.HttpContexts;

namespace cCoder.ContentManagement.Services.Orchestrations.PageContexts;

internal sealed partial class PageContextOrchestrationService(
    IHttpContextService httpContextService,
    IPageAuthorizationService pageAuthorizationService)
        : IPageContextOrchestrationService
{
    public ValueTask<HttpPageRenderContext>
        ResolvePageRenderContextAsync() =>
        TryCatch(operation: async () =>
    {
        ValidateResolvePageRenderContextAsync(inputs: []);

        HttpPageRenderContext context =
            httpContextService.GetPageRenderContext();

        return await pageAuthorizationService
            .AuthorizeHttpPageRenderContextAsync(
                pageRenderContext: context);

    }, isValueTask: true);
}