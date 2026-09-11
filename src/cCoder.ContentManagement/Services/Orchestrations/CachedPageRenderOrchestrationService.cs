// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal sealed partial class CachedPageRenderOrchestrationService(
    IPageRenderCacheQueryProcessingService queryProcessingService,
    ICachedPageRenderProcessingService renderProcessingService)
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
}