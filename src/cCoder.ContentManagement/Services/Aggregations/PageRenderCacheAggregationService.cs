// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;
using cCoder.ContentManagement.Exposures.Caching;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Aggregations;

internal sealed partial class PageRenderCacheAggregationService(
    IPageRenderCacheOrchestrationService pageRenderCacheOrchestrationService,
    PageRenderCacheImportState pageRenderCacheImportState,
    ICommonObjectCache commonObjectCache)
        : IPageRenderCacheAggregationService
{
    private static readonly HashSet<string> CommonCacheRenderTypes =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "Component",
            "Components",
            "Resource",
            "Resources",
            "Script",
            "Scripts"
        };

    public IQueryable<PageRenderCache> GetAllPageRenderCaches() =>
        TryCatch(operation: () =>
            pageRenderCacheOrchestrationService.GetAllPageRenderCaches());

    public PageRenderCache GetPageRenderCache(string pageRenderCacheId) =>
        TryCatch(operation: () =>
        {
            ValidatePageRenderCacheOnGet(inputs: [pageRenderCacheId]);

            return pageRenderCacheOrchestrationService.GetPageRenderCache(
                pageRenderCacheId: pageRenderCacheId);
        });

    public ValueTask<PageRenderCache> AddPageRenderCacheAsync(
        PageRenderCache newPageRenderCache) =>
        TryCatch<PageRenderCache>(operation: () =>
        {
            ValidatePageRenderCacheOnAdd(inputs: [newPageRenderCache]);

            return pageRenderCacheOrchestrationService.AddPageRenderCacheAsync(
                newPageRenderCache: newPageRenderCache);
        }, isValueTask: true);

    public ValueTask<PageRenderCache> UpdatePageRenderCacheAsync(
        PageRenderCache updatedPageRenderCache) =>
        TryCatch<PageRenderCache>(operation: () =>
        {
            ValidatePageRenderCacheOnUpdate(inputs: [updatedPageRenderCache]);

            return pageRenderCacheOrchestrationService.UpdatePageRenderCacheAsync(
                updatedPageRenderCache: updatedPageRenderCache);
        }, isValueTask: true);

    public ValueTask DeletePageRenderCacheAsync(string pageRenderCacheId) =>
        TryCatch(operation: () =>
        {
            ValidatePageRenderCacheOnDelete(inputs: [pageRenderCacheId]);

            return pageRenderCacheOrchestrationService.DeletePageRenderCacheAsync(
                pageRenderCacheId: pageRenderCacheId);
        }, isValueTask: true);

    public ValueTask DeleteAppAsync(
        int appId,
        bool fromEvent = false) =>
        TryCatch(operation: () =>
        {
            ValidateAppPageRenderCachesOnDelete(inputs: [appId, fromEvent]);

            return fromEvent
                ? DeleteAppPageRenderCacheFromEventAsync(appId: appId)
                : DeleteAppPageRenderCacheAsync(appId: appId);
        }, isValueTask: true);

    public ValueTask DeletePageAsync(
        int pageId,
        bool fromEvent = false) =>
        TryCatch(operation: () =>
        {
            ValidatePagePageRenderCachesOnDelete(inputs: [pageId, fromEvent]);

            return fromEvent
                ? DeletePagePageRenderCacheFromEventAsync(pageId: pageId)
                : DeletePagePageRenderCacheAsync(pageId: pageId);
        }, isValueTask: true);

    public ValueTask InvalidateCommonObjectConsumersAsync(
        string commonObjectType,
        bool fromEvent = false) =>
        TryCatch(operation: async () =>
        {
            ValidateCommonObjectPageRenderCachesOnRebuild(
                inputs: [commonObjectType, fromEvent]);

            if (!IsCommonCacheRenderType(type: commonObjectType))
            {
                return;
            }

            commonObjectCache.Refresh();

            int[] appIds =
            [
                .. pageRenderCacheOrchestrationService
                    .GetAllPageRenderCaches()
                    .Select(selector: cache => cache.AppId)
                    .Distinct()
            ];

            foreach (int appId in appIds)
            {
                if (fromEvent)
                {
                    await DeleteAppPageRenderCacheFromEventAsync(
                        appId: appId);
                }
                else
                {
                    await DeleteAppPageRenderCacheAsync(appId: appId);
                }
            }
        }, isValueTask: true);

    private ValueTask DeleteAppPageRenderCacheAsync(int appId) =>
        TryCatch(operation: () =>
        {
            ValidatePageRenderCacheByAppIdOnDelete(inputs: [appId]);

            return pageRenderCacheOrchestrationService
                .DeleteAppPageRenderCachesAsync(appId: appId);
        }, isValueTask: true);

    private ValueTask DeleteAppPageRenderCacheFromEventAsync(int appId) =>
        TryCatch(operation: () =>
        {
            ValidatePageRenderCacheByAppIdOnDelete(inputs: [appId]);

            return pageRenderCacheImportState.Active
                ? ValueTask.CompletedTask
                : pageRenderCacheOrchestrationService
                    .DeleteAppPageRenderCachesFromEventAsync(appId: appId);
        }, isValueTask: true);

    private ValueTask DeletePagePageRenderCacheAsync(int pageId) =>
        TryCatch(operation: () =>
        {
            ValidatePageRenderCacheByPageIdOnDelete(inputs: [pageId]);

            return pageRenderCacheOrchestrationService
                .DeletePagePageRenderCachesAsync(pageId: pageId);
        }, isValueTask: true);

    private ValueTask DeletePagePageRenderCacheFromEventAsync(int pageId) =>
        TryCatch(operation: () =>
        {
            ValidatePageRenderCacheByPageIdOnDelete(inputs: [pageId]);

            return pageRenderCacheImportState.Active
                ? ValueTask.CompletedTask
                : pageRenderCacheOrchestrationService
                    .DeletePagePageRenderCachesFromEventAsync(pageId: pageId);
        }, isValueTask: true);

    private static bool IsCommonCacheRenderType(string type)
    {
        string normalizedType = type?
            .Split(separator: '/', options: StringSplitOptions.RemoveEmptyEntries)
            .LastOrDefault() ?? string.Empty;

        return CommonCacheRenderTypes.Contains(item: normalizedType);
    }
}