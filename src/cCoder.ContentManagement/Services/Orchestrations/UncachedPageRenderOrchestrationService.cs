// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal sealed partial class UncachedPageRenderOrchestrationService(
    IPageRenderDataProcessingService pageRenderDataProcessingService,
    IPageRenderProcessingService pageRenderProcessingService)
        : IUncachedPageRenderOrchestrationService
{
    public ValueTask PrepareHttpPageRenderOperationAsync(
        HttpPageRenderOperation httpPageRenderOperation) =>
        TryCatch(operation: async () =>
    {
        ValidateHttpPageRenderOperationOnRenderAsync(inputs: [httpPageRenderOperation]);

        HttpPageRenderContext context = httpPageRenderOperation.Context;

        if (context.PageId is null)
        {
            httpPageRenderOperation.Failure =
                HttpPageRenderFailure.PageNotFound;

            return;
        }

        if (context.AccessDenied)
        {
            httpPageRenderOperation.Failure =
                HttpPageRenderFailure.PageAccessDenied;

            return;
        }

        Page page = await pageRenderDataProcessingService.GetPageForRenderAsync(
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

        httpPageRenderOperation.RenderOperation = renderOperation;

    }, isValueTask: true);

    public ValueTask CompleteHttpPageRenderOperationAsync(
        HttpPageRenderOperation httpPageRenderOperation) =>
        TryCatch(operation: () =>
    {
        ValidateHttpPageRenderOperationOnRenderAsync(inputs: [httpPageRenderOperation]);

        if (httpPageRenderOperation.Failure != HttpPageRenderFailure.None)
        {
            return ValueTask.CompletedTask;
        }

        HttpPageRenderContext context = httpPageRenderOperation.Context;
        PageRenderOperation renderOperation = httpPageRenderOperation.RenderOperation;
        Page page = renderOperation.SourcePage;
        string culture = renderOperation.Culture;
        string theme = renderOperation.Theme;

        renderOperation = pageRenderProcessingService
            .CompletePageRenderOperation(operation: renderOperation);

        httpPageRenderOperation.Response = new PageRenderResponse
        {
            App = page.App,
            Page = renderOperation.Page,
            Culture = culture,
            Theme = theme,
            Edit = context.Edit
        };

        return ValueTask.CompletedTask;

    }, isValueTask: true);

}