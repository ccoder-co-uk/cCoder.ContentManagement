// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Exposures;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Aggregations;

public interface IPageRenderAggregationService
{
    PageRenderResponse RenderPageRenderRequestPageRenderResponse(PageRenderRequest request);

    PageRenderResponse RenderErrorPageRenderRequestPageRenderResponse(PageRenderRequest request);

    RenderResult RenderRenderResult(int appId, string path, string theme, string culture, bool edit = false);
}