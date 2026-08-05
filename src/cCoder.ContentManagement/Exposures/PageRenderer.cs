// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Aggregations;

namespace cCoder.ContentManagement.Exposures;

internal sealed class PageRenderer(
    IRenderAggregationService renderAggregationService)
        : IPageRenderer
{
    public ValueTask<PageRenderResponse> RenderAsync() =>
        ExecuteRenderAsync();

    private async ValueTask<PageRenderResponse> ExecuteRenderAsync() =>
        (PageRenderResponse)await renderAggregationService
            .RenderPageRenderResultAsync();
}