// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Exposures;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal sealed partial class PageRenderCacheProcessingService(
    IPageRenderCacheService service,
    IAuthorizationManager authorizationManager) : IPageRenderCacheProcessingService
{
    public ValueTask<PageRenderCache> AddPageRenderCacheAsync(PageRenderCache newPageRenderCache) =>
        TryCatch<PageRenderCache>(operation: () =>
    {
        ValidatePageRenderCacheOnAdd(inputs: [newPageRenderCache]);
        ValidatePageRenderCache(cache: newPageRenderCache);
        authorizationManager.Authorize(appId: newPageRenderCache.AppId, privilege: "pagerendercache_create");
        NormalizeKey(cache: newPageRenderCache);
        return service.AddPageRenderCacheAsync(newPageRenderCache: newPageRenderCache);
    }, isValueTask: true);

    public ValueTask<PageRenderCache> UpdatePageRenderCacheAsync(PageRenderCache updatedPageRenderCache) =>
        TryCatch<PageRenderCache>(operation: () =>
    {
        ValidatePageRenderCacheOnUpdate(inputs: [updatedPageRenderCache]);
        ValidatePageRenderCache(cache: updatedPageRenderCache);
        ValidateId(pageRenderCacheId: updatedPageRenderCache.Id);
        authorizationManager.Authorize(appId: updatedPageRenderCache.AppId, privilege: "pagerendercache_update");
        NormalizeKey(cache: updatedPageRenderCache);
        return service.UpdatePageRenderCacheAsync(updatedPageRenderCache: updatedPageRenderCache);
    }, isValueTask: true);

    public ValueTask DeletePageRenderCacheAsync(int pageRenderCacheId) =>
        TryCatch(operation: () =>
    {
        ValidatePageRenderCacheOnDelete(inputs: [pageRenderCacheId]);

        PageRenderCache cache = service.GetPageRenderCache(
            pageRenderCacheId: ValidateId(pageRenderCacheId: pageRenderCacheId));

        if (cache == null)
        {
            return ValueTask.CompletedTask;
        }

        authorizationManager.Authorize(appId: cache.AppId, privilege: "pagerendercache_delete");
        return service.DeletePageRenderCacheAsync(pageRenderCacheId: pageRenderCacheId);
    }, isValueTask: true);

    public ValueTask ReplacePageRenderCachesAsync(int appId, int[] pageIds, PageRenderCache[] replacements) =>
        TryCatch(operation: () =>
    {
        ValidatePageRenderCachesOnReplace(inputs: [appId, pageIds, replacements]);
        ValidateId(pageRenderCacheId: appId);
        ArgumentNullException.ThrowIfNull(argument: replacements);
        authorizationManager.Authorize(appId: appId, privilege: "pagerendercache_rebuild");

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

    private static void ValidatePageRenderCache(PageRenderCache cache)
    {
        ArgumentNullException.ThrowIfNull(argument: cache);
        ValidateId(pageRenderCacheId: cache.AppId);
        ValidateId(pageRenderCacheId: cache.PageId);
        ArgumentException.ThrowIfNullOrWhiteSpace(argument: cache.Theme);
        ArgumentNullException.ThrowIfNull(argument: cache.Value);
        ArgumentNullException.ThrowIfNull(argument: cache.HeaderValue);
    }

    private static void NormalizeKey(PageRenderCache cache)
    {
        cache.Culture = (cache.Culture ?? string.Empty).Trim()
            .ToLowerInvariant();

        cache.Theme = cache.Theme.Trim()
            .ToLowerInvariant();
    }
}