// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Rendering.Services.Processings;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal sealed partial class PageRenderCacheOrchestrationService(
    IPageRenderCacheProcessingService processingService,
    ICommonObjectLatestCacheProcessingService commonObjectLatestCacheProcessingService,
    IAuthorizationProcessingService authorizationProcessingService)
        : IPageRenderCacheOrchestrationService
{
    public void RefreshCommonObjectCache() =>
        TryCatch(operation: () =>
            commonObjectLatestCacheProcessingService.RefreshCommonObjects(
                changedCommonObjectCount: 1));

    public IQueryable<PageRenderCache> GetAllPageRenderCaches() =>
        TryCatch<IQueryable<PageRenderCache>>(operation: () =>
        {

            return processingService.GetAllPageRenderCaches();
        });

    public PageRenderCache GetPageRenderCache(string pageRenderCacheId) =>
        TryCatch<PageRenderCache>(operation: () =>
        {
            ValidatePageRenderCacheOnGet(inputs: [pageRenderCacheId]);

            return processingService.GetPageRenderCache(
                pageRenderCacheId: pageRenderCacheId);
        });

    public ValueTask<PageRenderCache> AddPageRenderCacheAsync(
        PageRenderCache newPageRenderCache) =>
        TryCatch<PageRenderCache>(operation: () =>
        {
            ValidatePageRenderCacheOnAdd(inputs: [newPageRenderCache]);

            Authorize(
                appId: newPageRenderCache.AppId,
                privilege: "pagerendercache_create");

            return processingService.AddPageRenderCacheAsync(
                newPageRenderCache: newPageRenderCache);
        }, isValueTask: true);

    public ValueTask<PageRenderCache> UpdatePageRenderCacheAsync(
        PageRenderCache updatedPageRenderCache) =>
        TryCatch<PageRenderCache>(operation: () =>
        {
            ValidatePageRenderCacheOnUpdate(inputs: [updatedPageRenderCache]);

            Authorize(
                appId: updatedPageRenderCache.AppId,
                privilege: "pagerendercache_update");

            return processingService.UpdatePageRenderCacheAsync(
                updatedPageRenderCache: updatedPageRenderCache);
        }, isValueTask: true);

    public ValueTask DeletePageRenderCacheAsync(string pageRenderCacheId) =>
        TryCatch(operation: () =>
        {
            ValidatePageRenderCacheOnDelete(inputs: [pageRenderCacheId]);

            PageRenderCache cache = processingService.GetPageRenderCache(
                pageRenderCacheId: pageRenderCacheId);

            if (cache is null)
            {
                return ValueTask.CompletedTask;
            }

            Authorize(
                appId: cache.AppId,
                privilege: "pagerendercache_delete");

            return processingService.DeletePageRenderCacheAsync(
                pageRenderCacheId: pageRenderCacheId);
        }, isValueTask: true);

    public ValueTask DeleteAppPageRenderCachesAsync(int appId) =>
        TryCatch(operation: () =>
        {
            ValidateAppPageRenderCachesOnDelete(inputs: [appId]);

            Authorize(
                appId: appId,
                privilege: "pagerendercache_rebuild");

            return DeleteAppPageRenderCaches(
                appId: appId,
                fromEvent: false);
        }, isValueTask: true);

    public ValueTask DeleteAppPageRenderCachesFromEventAsync(int appId) =>
        TryCatch(operation: () =>
        {
            ValidateAppPageRenderCachesFromEventOnDelete(inputs: [appId]);

            return DeleteAppPageRenderCaches(
                appId: appId,
                fromEvent: true);
        }, isValueTask: true);

    private async ValueTask DeleteAppPageRenderCaches(
        int appId,
        bool fromEvent)
    {
        int[] pageIds = processingService.GetAllPageRenderCaches()
            .Where(predicate: cache => cache.AppId == appId)
            .Select(selector: cache => cache.PageId)
            .Distinct()
            .ToArray();

        foreach (int pageId in pageIds)
        {
            if (fromEvent)
            {
                await processingService.ReplacePageRenderCachesFromEventAsync(
                    appId: appId,
                    pageIds: [pageId],
                    replacements: []);
            }
            else
            {
                await processingService.ReplacePageRenderCachesAsync(
                    appId: appId,
                    pageIds: [pageId],
                    replacements: []);
            }
        }
    }

    public ValueTask DeletePagePageRenderCachesAsync(int pageId) =>
        TryCatch(operation: () =>
        {
            ValidatePagePageRenderCachesOnDelete(inputs: [pageId]);

            return DeletePagePageRenderCaches(
                pageId: pageId,
                fromEvent: false);
        }, isValueTask: true);

    public ValueTask DeletePagePageRenderCachesFromEventAsync(int pageId) =>
        TryCatch(operation: () =>
        {
            ValidatePagePageRenderCachesFromEventOnDelete(inputs: [pageId]);

            return DeletePagePageRenderCaches(
                pageId: pageId,
                fromEvent: true);
        }, isValueTask: true);

    private ValueTask DeletePagePageRenderCaches(
        int pageId,
        bool fromEvent)
    {
        PageRenderCache cache = processingService.GetAllPageRenderCaches()
            .FirstOrDefault(predicate: item => item.PageId == pageId);

        if (cache is null)
        {
            return ValueTask.CompletedTask;
        }

        if (fromEvent)
        {
            return processingService.ReplacePageRenderCachesFromEventAsync(
                appId: cache.AppId,
                pageIds: [pageId],
                replacements: []);
        }

        Authorize(
            appId: cache.AppId,
            privilege: "pagerendercache_rebuild");

        return processingService.ReplacePageRenderCachesAsync(
            appId: cache.AppId,
            pageIds: [pageId],
            replacements: []);
    }

    public ValueTask ReplacePageRenderCachesAsync(
        int appId,
        int[] pageIds,
        PageRenderCache[] replacements) =>
        TryCatch(operation: () =>
        {
            ValidatePageRenderCachesOnReplace(
                inputs: [appId, pageIds, replacements]);

            Authorize(
                appId: appId,
                privilege: "pagerendercache_rebuild");

            return processingService.ReplacePageRenderCachesAsync(
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
            ValidatePageRenderCachesOnReplace(
                inputs: [appId, pageIds, replacements]);

            return processingService.ReplacePageRenderCachesFromEventAsync(
                appId: appId,
                pageIds: pageIds,
                replacements: replacements);
        }, isValueTask: true);

    private void Authorize(int appId, string privilege) =>
        authorizationProcessingService.AuthorizeAuthorizationContext(
            context: new AuthorizationContext
            {
                Request = new AuthorizationRequest
                {
                    AppId = appId,
                    Privilege = privilege
                }
            });
}