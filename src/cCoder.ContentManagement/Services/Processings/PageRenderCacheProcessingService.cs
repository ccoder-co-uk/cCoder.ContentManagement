// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Exposures;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal sealed partial class PageRenderCacheProcessingService(
    IPageRenderCacheService service) : IPageRenderCacheProcessingService
{
    public IQueryable<PageRenderCache> GetAllPageRenderCaches() =>
        TryCatch<IQueryable<PageRenderCache>>(operation: () =>
            service.GetAllPageRenderCaches());

    public PageRenderCache GetPageRenderCache(string pageRenderCacheId) =>
        TryCatch<PageRenderCache>(operation: () =>
        {
            ValidatePageRenderCacheOnGet(inputs: [pageRenderCacheId]);
            ValidateId(pageRenderCacheId: pageRenderCacheId);

            return service.GetPageRenderCache(
                pageRenderCacheId: pageRenderCacheId);
        });

    public ValueTask<PageRenderCache> AddPageRenderCacheAsync(PageRenderCache newPageRenderCache) =>
        TryCatch<PageRenderCache>(operation: () =>
    {
        ValidatePageRenderCacheOnAdd(inputs: [newPageRenderCache]);
        ValidatePageRenderCache(cache: newPageRenderCache);
        NormalizeKey(cache: newPageRenderCache);
        return service.AddPageRenderCacheAsync(newPageRenderCache: newPageRenderCache);
    }, isValueTask: true);

    public ValueTask<PageRenderCache> UpdatePageRenderCacheAsync(PageRenderCache updatedPageRenderCache) =>
        TryCatch<PageRenderCache>(operation: () =>
    {
        ValidatePageRenderCacheOnUpdate(inputs: [updatedPageRenderCache]);
        ValidatePageRenderCache(cache: updatedPageRenderCache);
        ValidateId(pageRenderCacheId: updatedPageRenderCache.Id);
        NormalizeKey(cache: updatedPageRenderCache);
        return service.UpdatePageRenderCacheAsync(updatedPageRenderCache: updatedPageRenderCache);
    }, isValueTask: true);

    public ValueTask<PageRenderCache> StorePageRenderCacheAsync(
        PageRenderCache pageRenderCache) =>
        TryCatch<PageRenderCache>(operation: () =>
    {
        ArgumentNullException.ThrowIfNull(argument: pageRenderCache);
        NormalizeKey(cache: pageRenderCache);
        ValidatePageRenderCache(cache: pageRenderCache);

        return service.StorePageRenderCacheAsync(
            pageRenderCache: pageRenderCache);
    }, isValueTask: true);

    public ValueTask DeletePageRenderCacheAsync(string pageRenderCacheId) =>
        TryCatch(operation: () =>
    {
        ValidatePageRenderCacheOnDelete(inputs: [pageRenderCacheId]);

        PageRenderCache cache = service.GetPageRenderCache(
            pageRenderCacheId: ValidateId(pageRenderCacheId: pageRenderCacheId));

        if (cache == null)
        {
            return ValueTask.CompletedTask;
        }

        return service.DeletePageRenderCacheAsync(pageRenderCacheId: pageRenderCacheId);
    }, isValueTask: true);

    public ValueTask ReplacePageRenderCachesAsync(int appId, int[] pageIds, PageRenderCache[] replacements) =>
        TryCatch(operation: () =>
    {
        ValidatePageRenderCachesOnReplace(inputs: [appId, pageIds, replacements]);
        ValidateId(pageRenderCacheId: appId);
        ArgumentNullException.ThrowIfNull(argument: replacements);

        return ReplacePageRenderCaches(
            appId: appId,
            pageIds: pageIds,
            replacements: replacements);
    }, isValueTask: true);

    public ValueTask ReplacePageRenderCachesFromEventAsync(
        int appId,
        int[] pageIds,
        PageRenderCache[] replacements) =>
        TryCatch(operation: () =>
    {
        ValidatePageRenderCachesOnReplace(inputs: [appId, pageIds, replacements]);
        ValidateId(pageRenderCacheId: appId);
        ArgumentNullException.ThrowIfNull(argument: replacements);

        return ReplacePageRenderCaches(
            appId: appId,
            pageIds: pageIds,
            replacements: replacements);
    }, isValueTask: true);

    private ValueTask ReplacePageRenderCaches(
        int appId,
        int[] pageIds,
        PageRenderCache[] replacements)
    {

        foreach (PageRenderCache replacement in replacements)
        {
            ValidatePageRenderCache(cache: replacement);
            NormalizeKey(cache: replacement);
        }

        return service.ReplacePageRenderCachesAsync(
            appId: appId,
            pageIds: pageIds,
            replacements: replacements);
    }

    private static int ValidateId(int pageRenderCacheId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value: pageRenderCacheId);
        return pageRenderCacheId;
    }

    private static string ValidateId(string pageRenderCacheId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(argument: pageRenderCacheId);
        return pageRenderCacheId;
    }

    private static void ValidatePageRenderCache(PageRenderCache cache)
    {
        ArgumentNullException.ThrowIfNull(argument: cache);
        ValidateId(pageRenderCacheId: cache.Id);
        ValidateId(pageRenderCacheId: cache.AppId);
        ValidateId(pageRenderCacheId: cache.PageId);
        ArgumentException.ThrowIfNullOrWhiteSpace(argument: cache.Theme);
        ArgumentNullException.ThrowIfNull(argument: cache.Path);
        ArgumentNullException.ThrowIfNull(argument: cache.Header);
        ArgumentNullException.ThrowIfNull(argument: cache.Body);
    }

    private static void NormalizeKey(PageRenderCache cache)
    {
        cache.Culture = (cache.Culture ?? string.Empty).Trim()
            .ToLowerInvariant();

        cache.Theme = cache.Theme.Trim()
            .ToLowerInvariant();

        cache.Id = $"{cache.AppId}_{cache.PageId}_{cache.Culture}_{cache.Theme}";
    }
}