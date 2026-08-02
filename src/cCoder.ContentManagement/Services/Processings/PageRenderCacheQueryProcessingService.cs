// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal sealed partial class PageRenderCacheQueryProcessingService(
    IPageRenderCacheService pageRenderCacheService)
        : IPageRenderCacheQueryProcessingService
{
    public IQueryable<PageRenderCache> GetAllPageRenderCaches() =>
        TryCatch<IQueryable<PageRenderCache>>(operation: () =>
        {
            ValidateAllPageRenderCachesOnGet(inputs: []);

            return pageRenderCacheService.GetAllPageRenderCaches();
        });

    public PageRenderCache GetPageRenderCache(int pageRenderCacheId) =>
        TryCatch<PageRenderCache>(operation: () =>
        {
            ValidatePageRenderCacheOnGet(inputs: [pageRenderCacheId]);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value: pageRenderCacheId);

            return pageRenderCacheService.GetPageRenderCache(
                pageRenderCacheId: pageRenderCacheId);
        });
}