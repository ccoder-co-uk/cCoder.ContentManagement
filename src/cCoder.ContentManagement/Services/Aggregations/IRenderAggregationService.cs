// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Exposures;

namespace cCoder.ContentManagement.Services.Aggregations;

public interface IRenderAggregationService : IRenderer
{
    ValueTask<RenderResult> RenderPageRenderResultAsync();
}