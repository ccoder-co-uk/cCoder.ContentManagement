// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Aggregations;

public interface IPageRenderAggregationService
{
    PageRenderOperation RenderPageRenderOperation(
        PageRenderOperation operation);
}