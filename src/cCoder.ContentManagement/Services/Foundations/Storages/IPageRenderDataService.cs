// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal interface IPageRenderDataService
{
    ValueTask<PageRenderData> GetPageRenderDataAsync(int pageId);
}