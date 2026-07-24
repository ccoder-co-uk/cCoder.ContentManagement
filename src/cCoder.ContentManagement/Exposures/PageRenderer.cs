// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Aggregations;

namespace cCoder.ContentManagement.Exposures;

internal sealed class PageRenderer(
    IPageRenderAggregationService pageRenderAggregationService)
        : IPageRenderer
{
    public PageRenderResponse Render(PageRenderRequest request) =>
        pageRenderAggregationService.RenderPageRenderRequestPageRenderResponse(
            request: request);

    public PageRenderResponse RenderError(PageRenderRequest request) =>
        pageRenderAggregationService.RenderErrorPageRenderRequestPageRenderResponse(
            request: request);
}