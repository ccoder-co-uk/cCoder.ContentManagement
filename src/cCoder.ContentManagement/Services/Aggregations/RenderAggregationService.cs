// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Models.Exceptions;
using cCoder.ContentManagement.Models.PageRendering;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Services.Orchestrations.PageContexts;
using cCoder.ContentManagement.Brokers.Rendering;

namespace cCoder.ContentManagement.Services.Aggregations;

internal sealed partial class RenderAggregationService(
    IPageContextOrchestrationService pageContextOrchestrationService,
    ICachedPageRenderOrchestrationService cachedPageRenderOrchestrationService,
    IRenderEventOrchestrationService renderEventOrchestrationService,
    IRenderDataOrchestrationService renderDataOrchestrationService,
    ITemplateRenderOrchestrationService templateRenderOrchestrationService,
    IComponentRenderOrchestrationService componentRenderOrchestrationService)
        : IRenderAggregationService
{
    public ValueTask<RenderResult> RenderPageRenderResultAsync() =>
        TryCatch<RenderResult>(operation: async () =>
    {

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
            await renderEventOrchestrationService
                .RaiseHttpPageRenderOperationRenderRequestAsync(
                    httpPageRenderOperation: operation);

            ThrowRenderFailure(operation: operation);

            if (!context.Edit && context.PageId is not null)
            {
                operation = await cachedPageRenderOrchestrationService
                    .StoreHttpPageRenderOperationAsync(operation: operation);
            }
        }

        HydrateRequestValues(
            page: operation.Response.Page,
            context: context);

        return new RenderResult
        {
            PageResponse = operation.Response
        };

    }, isValueTask: true);

    private static void ThrowRenderFailure(HttpPageRenderOperation operation)
    {
        switch (operation.Failure)
        {
            case HttpPageRenderFailure.PageNotFound:
                throw new PageNotFoundException(
                    pageRenderContext: operation.Context);

            case HttpPageRenderFailure.PageAccessDenied:
                throw new PageAccessSecurityException(
                    pageRenderContext: operation.Context);
        }
    }

    private void HydrateRequestValues(
        PageRenderResult page,
        HttpPageRenderContext context)
    {
        bool isGuest = string.IsNullOrWhiteSpace(
                value: context.User?.Id)
            || string.Equals(
                a: context.User.Id,
                b: "Guest",
                comparisonType: StringComparison.OrdinalIgnoreCase);

        string displayName = isGuest
            ? "Guest"
            : context.User.DisplayName ?? context.User.Id;

        string loginLink = isGuest
            ? "<a href='/Login'>Login</a>"
            : "<a name='logout' href=''>Logout</a>";

        string serializedUser = renderDataOrchestrationService.SerializeRuntimeValue(value: new
        {
            Id = isGuest ? "Guest" : context.User.Id,
            DefaultCultureId = string.IsNullOrWhiteSpace(
                    value: context.User?.DefaultCultureId)
                ? context.Culture
                : context.User.DefaultCultureId,
            DisplayName = displayName,
            Email = context.User?.Email ?? string.Empty
        });

        page.HeaderHtml = HydrateMarkup(
            markup: page.HeaderHtml,
            context: context,
            serializedUser: serializedUser,
            displayName: displayName,
            loginLink: loginLink);

        page.BodyHtml = HydrateMarkup(
            markup: page.BodyHtml,
            context: context,
            serializedUser: serializedUser,
            displayName: displayName,
            loginLink: loginLink);
    }

    private string HydrateMarkup(
        string markup,
        HttpPageRenderContext context,
        string serializedUser,
        string displayName,
        string loginLink) =>
        (markup ?? string.Empty)
            .Replace(
                oldValue: ContentSecurityPolicyNonceContract.Placeholder,
                newValue: context.Nonce,
                comparisonType: StringComparison.Ordinal)
            .Replace(
                oldValue: PageRenderRuntimeTokens.User,
                newValue: serializedUser,
                comparisonType: StringComparison.Ordinal)
            .Replace(
                oldValue: PageRenderRuntimeTokens.DisplayName,
                newValue: renderDataOrchestrationService.HtmlEncode(value: displayName),
                comparisonType: StringComparison.Ordinal)
            .Replace(
                oldValue: PageRenderRuntimeTokens.LoginLink,
                newValue: loginLink,
                comparisonType: StringComparison.Ordinal)
            .Replace(
                oldValue: PageRenderRuntimeTokens.Date,
                newValue: DateTimeOffset.UtcNow.ToString(
                    format: "dd MMM yyyy"),
                comparisonType: StringComparison.Ordinal);

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