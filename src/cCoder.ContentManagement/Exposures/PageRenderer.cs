// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using System.Threading.Tasks;
using cCoder.CodeAnalysis.Exposures;
using cCoder.ContentManagement.Services.Aggregations;

namespace cCoder.ContentManagement.Exposures;

internal sealed class PageRenderer(
    IRenderAggregationService renderAggregationService)
        : IPageRenderer, ICompositionExposure
{
    public ValueTask<PageRenderResponse> RenderAsync() =>
        ExecuteRenderAsync();

    private async ValueTask<PageRenderResponse> ExecuteRenderAsync()
    {
        RenderResult result = await renderAggregationService
            .RenderPageRenderResultAsync();

        return result.PageResponse;
    }
}