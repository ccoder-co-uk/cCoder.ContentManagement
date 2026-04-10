using cCoder.ContentManagement.Services.Coordinations;

namespace cCoder.ContentManagement.Exposures;

internal sealed class PageRenderer(IPageRenderCoordinationService pageRenderCoordinationService) : IPageRenderer
{
    public PageRenderResponse Render(PageRenderRequest request) =>
        pageRenderCoordinationService.Render(request);

    public PageRenderResponse RenderError(PageRenderRequest request) =>
        pageRenderCoordinationService.RenderError(request);
}
