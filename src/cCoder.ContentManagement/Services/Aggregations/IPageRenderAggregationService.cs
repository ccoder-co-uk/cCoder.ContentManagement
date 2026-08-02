// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Aggregations;

internal interface IPageRenderAggregationService
{
    PageRenderOperation RenderPageRenderOperation(
        PageRenderOperation operation);

    ValueTask<PageRenderOperation> RebuildAppPageRenderOperationAsync(
        PageRenderOperation operation);

    ValueTask<PageRenderOperation> RebuildPagePageRenderOperationAsync(
        PageRenderOperation operation);

    ValueTask DeleteAppPageRenderCacheAsync(int appId);

    ValueTask DeleteAppPageRenderCacheFromEventAsync(int appId);

    ValueTask DeletePagePageRenderCacheAsync(int pageId);

    ValueTask DeletePagePageRenderCacheFromEventAsync(int pageId);

    ValueTask<PageRenderOperation> RebuildAllAppsPageRenderOperationAsync(
        PageRenderOperation operation);

    ValueTask<PageRenderOperation> RebuildCommonObjectPageRenderOperationAsync(
        PageRenderOperation operation);
}