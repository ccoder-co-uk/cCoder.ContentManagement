// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Exposures;

namespace cCoder.ContentManagement.Services.Aggregations;

internal interface IRenderAggregationService : IRenderer
{
    ValueTask<RenderResult> RenderPageRenderResultAsync();
}