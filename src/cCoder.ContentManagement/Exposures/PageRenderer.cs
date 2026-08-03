// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Aggregations;

namespace cCoder.ContentManagement.Exposures;

internal sealed class PageRenderer(
    IPageRenderAggregationService pageRenderAggregationService)
        : IPageRenderer
{
    public async ValueTask<PageRenderResponse> RenderAsync(PageRenderRequest request)
    {
        request.OperationType = PageRenderOperationType.Render;

        return await pageRenderAggregationService.RenderPageRenderOperationAsync(
            operation: request);
    }

    public async ValueTask<PageRenderResponse> RenderErrorAsync(PageRenderRequest request)
    {
        request.OperationType = PageRenderOperationType.RenderError;

        return await pageRenderAggregationService.RenderPageRenderOperationAsync(
            operation: request);
    }
}