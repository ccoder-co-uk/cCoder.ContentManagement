// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Exposures;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Coordinations;

public interface IPageRenderCoordinationService
{
    PageRenderResponse Render(PageRenderRequest request);

    PageRenderResponse RenderError(PageRenderRequest request);

    RenderResult RenderRenderResult(int appId, string path, string theme, string culture, bool edit = false);
}