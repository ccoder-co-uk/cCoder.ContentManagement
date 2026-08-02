// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Storages;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal sealed partial class PageRenderCacheService(IPageRenderCacheBroker broker) : IPageRenderCacheService
{
    public IQueryable<PageRenderCache> GetAllPageRenderCaches() =>
        TryCatch<IQueryable<PageRenderCache>>(operation: () =>
        {
            ValidateAllPageRenderCachesOnGet(inputs: []);

            return broker.GetAllPageRenderCaches();
        });

    public PageRenderCache GetPageRenderCache(int pageRenderCacheId) =>
        TryCatch<PageRenderCache>(operation: () =>
        {
            ValidatePageRenderCacheOnGet(inputs: [pageRenderCacheId]);
            ValidateId(pageRenderCacheId: pageRenderCacheId, parameterName: "id");

            return broker.GetAllPageRenderCaches()
                .FirstOrDefault(predicate: cache => cache.Id == pageRenderCacheId);
        });

    public ValueTask<PageRenderCache> AddPageRenderCacheAsync(PageRenderCache newPageRenderCache) =>
        TryCatch<PageRenderCache>(operation: () =>
        {
            ValidatePageRenderCacheOnAdd(inputs: [newPageRenderCache]);
            ValidatePageRenderCache(cache: newPageRenderCache, parameterName: "entity");

            return broker.AddPageRenderCacheAsync(
                newPageRenderCache: CreateStoragePageRenderCache(
                    pageRenderCache: newPageRenderCache));
        }, isValueTask: true);

    public ValueTask<PageRenderCache> UpdatePageRenderCacheAsync(PageRenderCache updatedPageRenderCache) =>
        TryCatch<PageRenderCache>(operation: () =>
        {
            ValidatePageRenderCacheOnUpdate(inputs: [updatedPageRenderCache]);
            ValidatePageRenderCache(cache: updatedPageRenderCache, parameterName: "entity");
            ValidateId(pageRenderCacheId: updatedPageRenderCache.Id, parameterName: "id");

            return broker.UpdatePageRenderCacheAsync(
                updatedPageRenderCache: CreateStoragePageRenderCache(
                    pageRenderCache: updatedPageRenderCache));
        }, isValueTask: true);

    public ValueTask DeletePageRenderCacheAsync(int pageRenderCacheId) =>
        TryCatch(operation: async () =>
        {
            ValidatePageRenderCacheOnDelete(inputs: [pageRenderCacheId]);
            ValidateId(pageRenderCacheId: pageRenderCacheId, parameterName: "id");

            PageRenderCache cache = broker.GetAllPageRenderCaches()
                .FirstOrDefault(predicate: cache => cache.Id == pageRenderCacheId);

            if (cache != null)
            {
                await broker.DeletePageRenderCacheAsync(deletedPageRenderCache: cache);
            }
        }, isValueTask: true);

    public ValueTask ReplacePageRenderCachesAsync(int appId, int[] pageIds, PageRenderCache[] replacements) =>
        TryCatch(operation: () =>
        {
            ValidatePageRenderCachesOnReplace(inputs: [appId, pageIds, replacements]);
            ValidateId(pageRenderCacheId: appId, parameterName: "appId");
            ValidateReplacementInputs(pageIds: pageIds, replacements: replacements);

            return pageIds == null
                ? broker.ReplacePageRenderCachesByAppIdAsync(
                    appId: appId,
                    replacements: replacements)
                : broker.ReplacePageRenderCachesByPageIdsAsync(
                    appId: appId,
                    pageIds: pageIds,
                    replacements: replacements);
        }, isValueTask: true);

    private static PageRenderCache CreateStoragePageRenderCache(PageRenderCache pageRenderCache) =>
        pageRenderCache == null
            ? null
            : new PageRenderCache
            {
                Id = pageRenderCache.Id,
                AppId = pageRenderCache.AppId,
                PageId = pageRenderCache.PageId,
                Culture = pageRenderCache.Culture,
                Theme = pageRenderCache.Theme,
                Value = pageRenderCache.Value,
                HeaderValue = pageRenderCache.HeaderValue,
                SourceFingerprint = pageRenderCache.SourceFingerprint,
                RenderedOn = pageRenderCache.RenderedOn
            };
}