// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Services.Aggregations;

internal interface IPageRenderCacheBuildAggregationService
{
    ValueTask BuildPageAsync(int pageId);
}