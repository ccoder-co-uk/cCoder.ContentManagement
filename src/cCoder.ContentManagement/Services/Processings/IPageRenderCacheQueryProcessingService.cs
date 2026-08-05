// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal interface IPageRenderCacheQueryProcessingService
{
    IQueryable<PageRenderCache> GetAllPageRenderCaches();

    PageRenderCache GetPageRenderCache(string pageRenderCacheId);

    PageRenderCache GetPageRenderCache(
        int pageId,
        string culture,
        string theme);
}