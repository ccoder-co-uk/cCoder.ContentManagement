// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal sealed partial class PageRenderCacheOrchestrationService(
    IPageRenderCacheProcessingService processingService,
    IAuthorizationProcessingService authorizationProcessingService)
        : IPageRenderCacheOrchestrationService
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

    public ValueTask InvalidateCommonObjectConsumersAsync(
        string commonObjectType) =>
        TryCatch(operation: () =>
    {
        ValidateCommonObjectConsumersOnInvalidate(
            inputs: [commonObjectType]);

        return IsCommonCacheRenderType(type: commonObjectType)
            ? ExecuteInvalidateCommonCacheAsync()
            : ValueTask.CompletedTask;
    }, isValueTask: true);

    public ValueTask InvalidateCommonCacheAsync() =>
        TryCatch(operation: async () =>
    {
        ValidateCommonCacheOnInvalidate(inputs: ["CommonCache"]);

        await ExecuteInvalidateCommonCacheAsync();
    }, isValueTask: true);

    private async ValueTask ExecuteInvalidateCommonCacheAsync()
    {

        int[] appIds = processingService
            .GetAllPageRenderCaches()
            .Select(selector: cache => cache.AppId)
            .Distinct()
            .ToArray();

        foreach (int appId in appIds)
        {
            await DeleteAppPageRenderCaches(
                appId: appId,
                fromEvent: true);
        }
    }

    public ValueTask InvalidatePackageAsync(int? appId) =>
        TryCatch(operation: () =>
    {
        ValidatePackageOnInvalidate(inputs: [appId]);

        return appId is int resolvedAppId
            ? DeleteAppPageRenderCaches(
                appId: resolvedAppId,
                fromEvent: true)
            : ExecuteInvalidateCommonCacheAsync();
    }, isValueTask: true);

    private static bool IsCommonCacheRenderType(string type)
    {
        string normalizedType = type?
            .Split(separator: '/', options: StringSplitOptions.RemoveEmptyEntries)
            .LastOrDefault() ?? string.Empty;

        return CommonCacheRenderTypes.Contains(item: normalizedType);
    }

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