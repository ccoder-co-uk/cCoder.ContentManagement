// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Services.Orchestrations.PageContexts;

namespace cCoder.ContentManagement.Services.Coordinations;

internal sealed partial class PageRenderCoordinationService(
    IPageContextOrchestrationService pageContextOrchestrationService,
    ICachedPageRenderOrchestrationService cachedPageRenderOrchestrationService,
    IUncachedPageRenderOrchestrationService uncachedPageRenderOrchestrationService)
        : IPageRenderCoordinationService
{
    public ValueTask<PageRenderResponse> RenderPageRenderResponseAsync() =>
        TryCatch<PageRenderResponse>(operation: async () =>
    {
        ValidateRenderPageRenderResponseAsync(inputs: []);

        HttpPageRenderContext context = await pageContextOrchestrationService
            .ResolvePageRenderContextAsync();

        HttpPageRenderOperation operation = new()
        {
            Context = context
        };

        if (!context.Edit
            && !context.AccessDenied
            && context.PageId is not null)
        {
            operation = await cachedPageRenderOrchestrationService
                .RenderHttpPageRenderOperationAsync(
                    operation: operation);
        }

        if (operation.Response is null)
        {
            operation = await uncachedPageRenderOrchestrationService
                .RenderHttpPageRenderOperationAsync(
                    operation: operation);
        }

        operation.Response.Page.HeaderHtml =
            operation.Response.Page.HeaderHtml.Replace(
                oldValue: ContentSecurityPolicyNonceContract.Placeholder,
                newValue: context.Nonce);

        operation.Response.Page.BodyHtml =
            operation.Response.Page.BodyHtml.Replace(
                oldValue: ContentSecurityPolicyNonceContract.Placeholder,
                newValue: context.Nonce);

        return operation.Response;

    }, isValueTask: true);
}