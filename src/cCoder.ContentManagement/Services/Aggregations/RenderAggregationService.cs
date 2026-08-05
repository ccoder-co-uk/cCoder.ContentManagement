// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Services.Orchestrations.PageContexts;

namespace cCoder.ContentManagement.Services.Aggregations;

internal sealed partial class RenderAggregationService(
    IPageContextOrchestrationService pageContextOrchestrationService,
    ICachedPageRenderOrchestrationService cachedPageRenderOrchestrationService,
    IUncachedPageRenderOrchestrationService uncachedPageRenderOrchestrationService,
    ITemplateRenderOrchestrationService templateRenderOrchestrationService,
    IComponentRenderOrchestrationService componentRenderOrchestrationService)
        : IRenderAggregationService
{
    public ValueTask<RenderResult> RenderPageRenderResultAsync() =>
        TryCatch<RenderResult>(operation: async () =>
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
            operation = cachedPageRenderOrchestrationService
                .RenderHttpPageRenderOperation(operation: operation);
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

    public ValueTask<RenderResult> RenderTemplateRenderResultAsync(
        string name,
        object model) =>
        TryCatch<RenderResult>(operation: async () =>
    {
        ValidateRenderTemplateRenderResult(
            inputs: [name, model]);

        HttpPageRenderContext context = await pageContextOrchestrationService
            .ResolvePageRenderContextAsync();

        return templateRenderOrchestrationService.RenderTemplateRenderResult(
            appId: context.AppId,
            name: name,
            culture: context.Culture,
            model: model);
    }, isValueTask: true);

    public ValueTask<RenderResult> RenderComponentRenderResultAsync(
        string name) =>
        TryCatch<RenderResult>(operation: async () =>
    {
        ValidateRenderComponentRenderResult(
            inputs: [name]);

        HttpPageRenderContext context = await pageContextOrchestrationService
            .ResolvePageRenderContextAsync();

        return componentRenderOrchestrationService.RenderComponentRenderResult(
            appId: context.AppId,
            name: name,
            culture: context.Culture,
            theme: context.Theme);
    }, isValueTask: true);
}