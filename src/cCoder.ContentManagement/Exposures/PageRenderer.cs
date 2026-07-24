// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Aggregations;

namespace cCoder.ContentManagement.Exposures;

internal sealed class PageRenderer(
    IPageRenderAggregationService pageRenderAggregationService)
        : IPageRenderer
{
    public PageRenderResponse Render(PageRenderRequest request)
    {
        request.OperationType = PageRenderOperationType.Render;

        return pageRenderAggregationService.RenderPageRenderOperation(
            operation: request);
    }

    public PageRenderResponse RenderError(PageRenderRequest request)
    {
        request.OperationType = PageRenderOperationType.RenderError;

        return pageRenderAggregationService.RenderPageRenderOperation(
            operation: request);
    }
}