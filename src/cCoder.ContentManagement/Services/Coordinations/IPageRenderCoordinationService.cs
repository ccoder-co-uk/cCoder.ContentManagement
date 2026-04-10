using cCoder.ContentManagement.Exposures;
using RenderResult = cCoder.ContentManagement.Models.RenderResult;

namespace cCoder.ContentManagement.Services.Coordinations;

public interface IPageRenderCoordinationService
{
    PageRenderResponse Render(PageRenderRequest request);

    PageRenderResponse RenderError(PageRenderRequest request);

    RenderResult Render(int appId, string path, string theme, string culture, bool edit = false);
}
