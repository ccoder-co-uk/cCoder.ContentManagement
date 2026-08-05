// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Coordinations;

namespace cCoder.ContentManagement.Exposures;

internal sealed class PageRenderer(
    IPageRenderCoordinationService pageRenderCoordinationService)
        : IPageRenderer
{
    public ValueTask<PageRenderResponse> RenderAsync() =>
        pageRenderCoordinationService.RenderPageRenderResponseAsync();
}