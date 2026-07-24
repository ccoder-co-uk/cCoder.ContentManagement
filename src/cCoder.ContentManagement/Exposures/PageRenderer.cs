// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Coordinations;

namespace cCoder.ContentManagement.Exposures;

internal sealed class PageRenderer(IPageRenderCoordinationService pageRenderCoordinationService) : IPageRenderer
{
    public PageRenderResponse Render(PageRenderRequest request) =>
        pageRenderCoordinationService.RenderPageRenderRequestPageRenderResponse(request: request);

    public PageRenderResponse RenderError(PageRenderRequest request) =>
        pageRenderCoordinationService.RenderErrorPageRenderRequestPageRenderResponse(request: request);
}