// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Models.Exceptions;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal sealed partial class UncachedPageRenderOrchestrationService(
    IPageProcessingService pageProcessingService,
    IPageRenderProcessingService pageRenderProcessingService,
    IUncachedPageRenderEventProcessingService eventProcessingService)
        : IUncachedPageRenderOrchestrationService
{
    public ValueTask<HttpPageRenderOperation>
        RenderHttpPageRenderOperationAsync(
            HttpPageRenderOperation operation) =>
        TryCatch<HttpPageRenderOperation>(operation: async () =>
    {
        ValidateHttpPageRenderOperationOnRenderAsync(inputs: [operation]);

        HttpPageRenderContext context = operation.Context;

        if (context.PageId is null)
        {
            throw new PageNotFoundException(
                pageRenderContext: context);
        }

        if (context.AccessDenied)
        {
            throw new PageAccessSecurityException(
                pageRenderContext: context);
        }

        Page page = await pageProcessingService.GetPageForRenderAsync(
            pageId: context.PageId.Value);

        string culture = string.IsNullOrWhiteSpace(value: context.Culture)
            ? page.App.DefaultCultureId ?? string.Empty
            : context.Culture;

        string theme = string.IsNullOrWhiteSpace(value: context.Theme)
            ? page.App.DefaultTheme ?? "Default"
            : context.Theme;

        PageRenderOperation renderOperation =
            pageRenderProcessingService.RenderPageRenderOperation(
                operation: new PageRenderOperation
                {
                    OperationType = PageRenderOperationType.RenderResult,
                    SourcePage = page,
                    User = context.User,
                    Theme = theme,
                    Culture = culture,
                    Edit = context.Edit
                });

        operation.Response = new PageRenderResponse
        {
            App = page.App,
            Page = renderOperation.Page,
            Culture = culture,
            Theme = theme,
            Edit = context.Edit
        };

        if (!context.Edit)
        {
            await eventProcessingService.RaiseUncachedPageRenderEventAsync(
                pageRenderEvent: new UncachedPageRenderEvent
                {
                    PageId = page.Id
                });
        }

        return operation;

    }, isValueTask: true);

}