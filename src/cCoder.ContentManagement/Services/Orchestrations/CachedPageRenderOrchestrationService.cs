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
        HttpPageRenderOperation operation) =>
        TryCatch(operation: () =>
        {
            ValidateHttpPageRenderOperationOnRender(inputs: [operation]);

            HttpPageRenderContext context = operation.Context;

            if (context.PageId is null)
            {
                return operation;
            }

            PageRenderCache cache = queryProcessingService.GetPageRenderCache(
                pageId: context.PageId.Value,
                culture: context.Culture,
                theme: context.Theme);

            if (cache is null)
            {
                return operation;
            }

            return renderProcessingService.RenderPageRenderCacheOperation(
                operation: new PageRenderCacheOperation
                {
                    Cache = cache,
                    RenderOperation = operation
                })
            .RenderOperation;
        });
}