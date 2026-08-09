// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal sealed partial class PageRenderCacheOrchestrationService(
    IPageRenderCacheQueryProcessingService queryProcessingService,
    IPageRenderCacheProcessingService processingService)
        : IPageRenderCacheOrchestrationService
{
    public IQueryable<PageRenderCache> GetAllPageRenderCaches() =>
        TryCatch<IQueryable<PageRenderCache>>(operation: () =>
        {
            ValidateAllPageRenderCachesOnGet(inputs: []);

            return queryProcessingService.GetAllPageRenderCaches();
        });

    public PageRenderCache GetPageRenderCache(string pageRenderCacheId) =>
        TryCatch<PageRenderCache>(operation: () =>
        {
            ValidatePageRenderCacheOnGet(inputs: [pageRenderCacheId]);

            return queryProcessingService.GetPageRenderCache(
                pageRenderCacheId: pageRenderCacheId);
        });

    public ValueTask<PageRenderCache> AddPageRenderCacheAsync(
        PageRenderCache newPageRenderCache) =>
        TryCatch<PageRenderCache>(operation: () =>
        {
            ValidatePageRenderCacheOnAdd(inputs: [newPageRenderCache]);

            return processingService.AddPageRenderCacheAsync(
                newPageRenderCache: newPageRenderCache);
        }, isValueTask: true);

    public ValueTask<PageRenderCache> UpdatePageRenderCacheAsync(
        PageRenderCache updatedPageRenderCache) =>
        TryCatch<PageRenderCache>(operation: () =>
        {
            ValidatePageRenderCacheOnUpdate(inputs: [updatedPageRenderCache]);

            return processingService.UpdatePageRenderCacheAsync(
                updatedPageRenderCache: updatedPageRenderCache);
        }, isValueTask: true);

    public ValueTask DeletePageRenderCacheAsync(string pageRenderCacheId) =>
        TryCatch(operation: () =>
        {
            ValidatePageRenderCacheOnDelete(inputs: [pageRenderCacheId]);

            return processingService.DeletePageRenderCacheAsync(
                pageRenderCacheId: pageRenderCacheId);
        }, isValueTask: true);

    public ValueTask DeleteAppPageRenderCachesAsync(int appId) =>
        TryCatch(operation: () =>
        {
            ValidatePageRenderCacheOnDelete(inputs: [appId]);

            return DeleteAppPageRenderCaches(
                appId: appId,
                fromEvent: false);
        }, isValueTask: true);

    public ValueTask DeleteAppPageRenderCachesFromEventAsync(int appId) =>
        TryCatch(operation: () =>
        {
            ValidatePageRenderCacheOnDelete(inputs: [appId]);

            return DeleteAppPageRenderCaches(
                appId: appId,
                fromEvent: true);
        }, isValueTask: true);

    private async ValueTask DeleteAppPageRenderCaches(
        int appId,
        bool fromEvent)
    {
        int[] pageIds = queryProcessingService.GetAllPageRenderCaches()
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
            ValidatePageRenderCacheOnDelete(inputs: [pageId]);

            return DeletePagePageRenderCaches(
                pageId: pageId,
                fromEvent: false);
        }, isValueTask: true);

    public ValueTask DeletePagePageRenderCachesFromEventAsync(int pageId) =>
        TryCatch(operation: () =>
        {
            ValidatePageRenderCacheOnDelete(inputs: [pageId]);

            return DeletePagePageRenderCaches(
                pageId: pageId,
                fromEvent: true);
        }, isValueTask: true);

    private ValueTask DeletePagePageRenderCaches(
        int pageId,
        bool fromEvent)
    {
        PageRenderCache cache = queryProcessingService.GetAllPageRenderCaches()
            .FirstOrDefault(predicate: item => item.PageId == pageId);

        return cache is null
            ? ValueTask.CompletedTask
            : fromEvent
                ? processingService.ReplacePageRenderCachesFromEventAsync(
                    appId: cache.AppId,
                    pageIds: [pageId],
                    replacements: [])
                : processingService.ReplacePageRenderCachesAsync(
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
}