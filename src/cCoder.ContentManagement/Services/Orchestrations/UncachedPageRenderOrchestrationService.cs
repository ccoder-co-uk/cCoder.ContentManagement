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
    IPageRenderCacheProcessingService pageRenderCacheProcessingService)
        : IUncachedPageRenderOrchestrationService
{
    public ValueTask<HttpPageRenderOperation>
        RenderHttpPageRenderOperationAsync(
            HttpPageRenderOperation httpPageRenderOperation) =>
        TryCatch<HttpPageRenderOperation>(operation: async () =>
    {
        ValidateHttpPageRenderOperationOnRenderAsync(inputs: [httpPageRenderOperation]);

        HttpPageRenderContext context = httpPageRenderOperation.Context;

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

        httpPageRenderOperation.Response = new PageRenderResponse
        {
            App = page.App,
            Page = renderOperation.Page,
            Culture = culture,
            Theme = theme,
            Edit = context.Edit
        };

        if (!context.Edit)
        {
            await pageRenderCacheProcessingService.StorePageRenderCacheAsync(
                pageRenderCache: CreatePageRenderCache(
                    response: httpPageRenderOperation.Response));
        }

        return httpPageRenderOperation;

    }, isValueTask: true);

    private PageRenderCache CreatePageRenderCache(
        PageRenderResponse response)
    {
        PageRenderResult result = response.Page;

        string fingerprintSource = pageRenderProcessingService
            .SerializeRuntimeValue(value: result);

        return new PageRenderCache
        {
            AppId = result.AppId,
            PageId = result.PageId,
            Culture = response.Culture,
            Theme = response.Theme,
            ParentId = result.ParentId,
            Path = result.Path,
            Title = result.Title,
            Description = result.Description,
            Keywords = result.Keywords,
            ShowOnMenus = result.ShowOnMenus,
            Header = result.HeaderHtml,
            Body = result.BodyHtml,
            SourceFingerprint = pageRenderProcessingService.ComputeFingerprint(
                value: fingerprintSource),
            RenderedOn = DateTimeOffset.UtcNow
        };
    }

}