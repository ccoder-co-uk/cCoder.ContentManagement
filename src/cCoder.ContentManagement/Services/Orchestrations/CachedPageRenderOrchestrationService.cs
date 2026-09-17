// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal sealed partial class CachedPageRenderOrchestrationService(
    IPageRenderCacheQueryProcessingService queryProcessingService,
    ICachedPageRenderProcessingService renderProcessingService,
    IPageRenderCacheProcessingService cacheProcessingService)
        : ICachedPageRenderOrchestrationService
{
    public HttpPageRenderOperation RenderHttpPageRenderOperation(
        HttpPageRenderOperation httpPageRenderOperation) =>
        TryCatch(operation: () =>
        {
            ValidateHttpPageRenderOperationOnRender(inputs: [httpPageRenderOperation]);

            HttpPageRenderContext context = httpPageRenderOperation.Context;

            if (context.PageId is null)
            {
                return httpPageRenderOperation;
            }

            PageRenderCache cache = queryProcessingService.GetPageRenderCache(
                pageId: context.PageId.Value,
                culture: context.Culture,
                theme: context.Theme);

            if (cache is null)
            {
                return httpPageRenderOperation;
            }

            return renderProcessingService.RenderPageRenderCacheOperation(
                operation: new PageRenderCacheOperation
                {
                    Cache = cache,
                    RenderOperation = httpPageRenderOperation
                })
            .RenderOperation;
        });

    public ValueTask<HttpPageRenderOperation> StoreHttpPageRenderOperationAsync(
        HttpPageRenderOperation httpPageRenderOperation) =>
        TryCatch<HttpPageRenderOperation>(operation: async () =>
        {
            ValidateHttpPageRenderOperationOnRender(
                inputs: [httpPageRenderOperation]);

            PageRenderResponse response = httpPageRenderOperation.Response;
            PageRenderResult result = response.Page;

            await cacheProcessingService.StorePageRenderCacheAsync(
                pageRenderCache: new PageRenderCache
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
                    RenderedOn = DateTimeOffset.UtcNow
                });

            return httpPageRenderOperation;
        }, isValueTask: true);
}