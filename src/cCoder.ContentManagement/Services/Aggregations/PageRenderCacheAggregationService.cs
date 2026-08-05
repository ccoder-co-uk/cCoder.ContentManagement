// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security.Cryptography;
using System.Text;
using cCoder.ContentManagement.Dependencies;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace cCoder.ContentManagement.Services.Aggregations;

internal sealed partial class PageRenderCacheAggregationService(
    IAppOrchestrationService appOrchestrationService,
    IPageOrchestrationService pageOrchestrationService,
    IPageRenderOrchestrationService pageRenderOrchestrationService,
    IPageRenderCacheOrchestrationService pageRenderCacheOrchestrationService,
    PageRenderCacheImportState pageRenderCacheImportState)
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
        {
            return pageRenderCacheOrchestrationService
                .GetAllPageRenderCaches();
        });

    public PageRenderCache GetPageRenderCache(string pageRenderCacheId) =>
        TryCatch(operation: () =>
        {
            ValidatePageRenderCacheOnGet(inputs: [pageRenderCacheId]);

            return pageRenderCacheOrchestrationService.GetPageRenderCache(
                pageRenderCacheId: pageRenderCacheId);
        });

    public ValueTask<PageRenderCache> AddPageRenderCacheAsync(
        PageRenderCache newPageRenderCache) =>
        TryCatch<PageRenderCache>(operation: async () =>
        {
            ValidatePageRenderCacheOnAdd(inputs: [newPageRenderCache]);

            return await pageRenderCacheOrchestrationService
                .AddPageRenderCacheAsync(
                    newPageRenderCache: newPageRenderCache);
        }, isValueTask: true);

    public ValueTask<PageRenderCache> UpdatePageRenderCacheAsync(
        PageRenderCache updatedPageRenderCache) =>
        TryCatch<PageRenderCache>(operation: async () =>
        {
            ValidatePageRenderCacheOnUpdate(inputs: [updatedPageRenderCache]);

            return await pageRenderCacheOrchestrationService
                .UpdatePageRenderCacheAsync(
                    updatedPageRenderCache: updatedPageRenderCache);
        }, isValueTask: true);

    public ValueTask DeletePageRenderCacheAsync(string pageRenderCacheId) =>
        TryCatch(operation: () =>
        {
            ValidatePageRenderCacheOnDelete(inputs: [pageRenderCacheId]);

            return pageRenderCacheOrchestrationService
                .DeletePageRenderCacheAsync(
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

    public ValueTask<PageRenderCache[]> RebuildAllAppsAsync(
        bool fromEvent = false) =>
        TryCatch<PageRenderCache[]>(operation: async () =>
        {
            ValidateAllPageRenderCachesOnRebuild(inputs: [fromEvent]);

            PageRenderOperation operation =
                await RebuildAllAppsPageRenderOperationAsync(
                    operation: new PageRenderOperation
                    {
                        RebuildCache = fromEvent
                    });

            return operation.PageRenderCaches;
        }, isValueTask: true);

    public ValueTask<PageRenderCache[]> RebuildCommonObjectConsumersAsync(
        string commonObjectType,
        bool fromEvent = false) =>
        TryCatch<PageRenderCache[]>(operation: async () =>
        {
            ValidateCommonObjectPageRenderCachesOnRebuild(
                inputs: [commonObjectType, fromEvent]);

            PageRenderOperation operation =
                await RebuildCommonObjectPageRenderOperationAsync(
                    operation: new PageRenderOperation
                    {
                        CommonObject = new CommonObject { Type = commonObjectType },
                        RebuildCache = fromEvent
                    });

            return operation.PageRenderCaches;
        }, isValueTask: true);

    public ValueTask<PageRenderCache[]> RebuildAppAsync(
        int appId,
        bool fromEvent = false) =>
        TryCatch<PageRenderCache[]>(operation: async () =>
        {
            ValidateAppPageRenderCachesOnRebuild(inputs: [appId, fromEvent]);

            PageRenderOperation operation =
                await RebuildAppPageRenderOperationAsync(
                    operation: new PageRenderOperation
                    {
                        AppId = appId,
                        RebuildCache = fromEvent
                    });

            return operation.PageRenderCaches;
        }, isValueTask: true);

    public ValueTask<PageRenderCache[]> RebuildPageAsync(
        int pageId,
        bool fromEvent = false) =>
        TryCatch<PageRenderCache[]>(operation: async () =>
        {
            ValidatePagePageRenderCachesOnRebuild(inputs: [pageId, fromEvent]);

            PageRenderOperation operation =
                await RebuildPagePageRenderOperationAsync(
                    operation: new PageRenderOperation
                    {
                        PageId = pageId,
                        RebuildCache = fromEvent
                    });

            return operation.PageRenderCaches;
        }, isValueTask: true);

    private ValueTask DeleteAppPageRenderCacheAsync(int appId) =>
        TryCatch(operation: () =>
        {
            ValidatePageRenderCacheByAppIdOnDelete(inputs: [appId]);

            return pageRenderCacheOrchestrationService.DeleteAppPageRenderCachesAsync(
                appId: appId);
        }, isValueTask: true);

    private ValueTask DeleteAppPageRenderCacheFromEventAsync(int appId) =>
        TryCatch(operation: () =>
        {
            ValidatePageRenderCacheByAppIdOnDelete(inputs: [appId]);

            if (pageRenderCacheImportState.Active)
            {
                return ValueTask.CompletedTask;
            }

            return pageRenderCacheOrchestrationService
                .DeleteAppPageRenderCachesFromEventAsync(appId: appId);
        }, isValueTask: true);

    private ValueTask DeletePagePageRenderCacheAsync(int pageId) =>
        TryCatch(operation: () =>
        {
            ValidatePageRenderCacheByPageIdOnDelete(inputs: [pageId]);

            return pageRenderCacheOrchestrationService.DeletePagePageRenderCachesAsync(
                pageId: pageId);
        }, isValueTask: true);

    private ValueTask DeletePagePageRenderCacheFromEventAsync(int pageId) =>
        TryCatch(operation: () =>
        {
            ValidatePageRenderCacheByPageIdOnDelete(inputs: [pageId]);

            if (pageRenderCacheImportState.Active)
            {
                return ValueTask.CompletedTask;
            }

            return pageRenderCacheOrchestrationService
                .DeletePagePageRenderCachesFromEventAsync(pageId: pageId);
        }, isValueTask: true);

    private ValueTask<PageRenderOperation> RebuildAllAppsPageRenderOperationAsync(
        PageRenderOperation operation) =>
        TryCatch<PageRenderOperation>(operation: async () =>
        {
            ValidateAllPageRenderCachesOnRebuild(inputs: [operation]);

            operation.PageRenderCaches = await ExecuteRebuildAllAppsPageRenderCacheAsync(
                fromEvent: false);

            return operation;
        }, isValueTask: true);

    private ValueTask<PageRenderOperation> RebuildCommonObjectPageRenderOperationAsync(
        PageRenderOperation operation) =>
        TryCatch<PageRenderOperation>(operation: async () =>
        {
            ValidateCommonObjectPageRenderOperationOnRebuild(inputs: [operation]);

            if (pageRenderCacheImportState.Active && operation.RebuildCache)
            {
                return operation;
            }

            operation.PageRenderCaches = IsCommonCacheRenderType(
                type: operation.CommonObject.Type)
                    ? await ExecuteRebuildAllAppsPageRenderCacheAsync(
                        fromEvent: operation.RebuildCache)
                    : [];

            return operation;
        }, isValueTask: true);

    private async ValueTask<PageRenderCache[]> ExecuteRebuildAllAppsPageRenderCacheAsync(
        bool fromEvent)
    {
        int[] appIds = fromEvent
            ?
            [
                .. pageRenderCacheOrchestrationService
                    .GetAllPageRenderCaches()
                    .Select(selector: cache => cache.AppId)
                    .Distinct()
            ]
            :
            [
                .. appOrchestrationService.GetAllApp(ignoreFilters: true)
                    .Select(selector: app => app.Id)
            ];

        List<PageRenderCache> rebuilt = [];

        foreach (int appId in appIds)
        {
            PageRenderOperation operation = await ExecuteRebuildAppPageRenderOperationAsync(
                operation: new PageRenderOperation
                {
                    AppId = appId,
                    RebuildCache = fromEvent
                });

            rebuilt.AddRange(collection: operation.PageRenderCaches);
        }

        return [.. rebuilt];
    }

    private static bool IsCommonCacheRenderType(string type)
    {
        string normalizedType = type?
            .Split(separator: '/', options: StringSplitOptions.RemoveEmptyEntries)
            .LastOrDefault() ?? string.Empty;

        return CommonCacheRenderTypes.Contains(item: normalizedType);
    }

    private ValueTask<PageRenderOperation> RebuildAppPageRenderOperationAsync(
        PageRenderOperation operation) =>
        TryCatch<PageRenderOperation>(operation: async () =>
        {
            ValidatePageRenderCachesByAppIdOnRebuild(inputs: [operation]);

            if (pageRenderCacheImportState.Active && operation.RebuildCache)
            {
                return operation;
            }

            return await ExecuteRebuildAppPageRenderOperationAsync(operation: operation);
        }, isValueTask: true);

    private async ValueTask<PageRenderOperation> ExecuteRebuildAppPageRenderOperationAsync(
        PageRenderOperation operation)
    {
        App app = await appOrchestrationService.GetAppForRenderAsync(
            appId: operation.AppId)
                ?? throw new KeyNotFoundException(
                    message: $"App {operation.AppId} was not found.");

        int[] pageIds = [.. app.Pages.Select(selector: page => page.Id)];

        List<PageRenderCache> rebuilt = [];

        foreach (int pageId in pageIds)
        {
            Page page = await pageOrchestrationService.GetPageForRenderAsync(
                pageId: pageId);

            rebuilt.AddRange(
                collection: BuildPageRenderCaches(
                    app: page.App,
                    page: page));
        }

        if (operation.RebuildCache)
        {
            await pageRenderCacheOrchestrationService
                .ReplacePageRenderCachesFromEventAsync(
                    appId: app.Id,
                    pageIds: pageIds,
                    replacements: [.. rebuilt]);
        }
        else
        {
            await pageRenderCacheOrchestrationService.ReplacePageRenderCachesAsync(
                appId: app.Id,
                pageIds: pageIds,
                replacements: [.. rebuilt]);
        }

        operation.PageRenderCaches = [.. rebuilt];
        return operation;
    }

    private ValueTask<PageRenderOperation> RebuildPagePageRenderOperationAsync(
        PageRenderOperation operation) =>
        TryCatch<PageRenderOperation>(operation: async () =>
        {
            ValidatePageRenderCachesByPageIdOnRebuild(inputs: [operation]);

            if (pageRenderCacheImportState.Active && operation.RebuildCache)
            {
                return operation;
            }

            Page page = await pageOrchestrationService.GetPageForRenderAsync(
                pageId: operation.PageId)
                    ?? throw new KeyNotFoundException(
                        message: $"Page {operation.PageId} was not found.");

            App app = page.App;

            operation.PageRenderCaches = BuildPageRenderCaches(
                app: app,
                page: page);

            if (operation.RebuildCache)
            {
                await pageRenderCacheOrchestrationService
                    .ReplacePageRenderCachesFromEventAsync(
                        appId: app.Id,
                        pageIds: [page.Id],
                        replacements: operation.PageRenderCaches);
            }
            else
            {
                await pageRenderCacheOrchestrationService.ReplacePageRenderCachesAsync(
                    appId: app.Id,
                    pageIds: [page.Id],
                    replacements: operation.PageRenderCaches);
            }

            return operation;
        }, isValueTask: true);

    private PageRenderCache[] BuildPageRenderCaches(
        App app,
        Page page)
    {
        string[] cultures = ResolvePageRenderCacheCultures(app: app);
        string[] themes = ResolvePageRenderCacheThemes(app: app);

        List<PageRenderCache> replacements = new(
            capacity: cultures.Length * themes.Length);

        foreach (string culture in cultures)
        {
            foreach (string theme in themes)
            {
                PageRenderOperation operation = pageRenderOrchestrationService
                    .ProcessPageRenderOperation(
                        operation: new PageRenderOperation
                        {
                            OperationType = PageRenderOperationType.RenderResult,
                            SourcePage = page,
                            Culture = culture,
                            Theme = theme,
                            Edit = false,
                            CacheTemplate = true
                        });

                PageRenderResult result = operation.Page as PageRenderResult
                    ?? throw new InvalidOperationException(
                        message: $"Page {page.Id} did not produce a render result.");

                if (result.Edit)
                {
                    throw new InvalidOperationException(
                        message: "Edit-mode results cannot be cached.");
                }

                string fingerprintSource = JsonConvert.SerializeObject(
                    value: result,
                    formatting: Formatting.None);

                string normalizedCulture = NormalizePageRenderCacheKey(
                    value: culture);

                string normalizedTheme = NormalizePageRenderCacheKey(
                    value: theme);

                replacements.Add(item: new PageRenderCache
                {
                    Id = CreatePageRenderCacheId(
                        appId: app.Id,
                        pageId: page.Id,
                        culture: normalizedCulture,
                        theme: normalizedTheme),
                    AppId = app.Id,
                    PageId = page.Id,
                    Culture = normalizedCulture,
                    Theme = normalizedTheme,
                    ParentId = result.ParentId,
                    Path = result.Path,
                    Title = result.Title,
                    Description = result.Description,
                    Keywords = result.Keywords,
                    ShowOnMenus = result.ShowOnMenus,
                    Header = result.HeaderHtml,
                    Body = result.BodyHtml,
                    SourceFingerprint = Convert.ToHexString(
                        inArray: SHA256.HashData(
                            source: Encoding.UTF8.GetBytes(s: fingerprintSource))),
                    RenderedOn = DateTimeOffset.UtcNow
                });
            }
        }

        return [.. replacements];
    }

    private string[] ResolvePageRenderCacheCultures(App app)
    {
        string[] cultures =
        [
            .. app.Cultures
                .Select(selector: appCulture => appCulture.CultureId)
                .Where(predicate: culture => culture.Length > 0)
                .AsEnumerable()
                .Select(selector: culture => culture.Trim())
                .Distinct(comparer: StringComparer.OrdinalIgnoreCase)
                .Prepend(element: string.Empty)
        ];

        return cultures;
    }

    private static string[] ResolvePageRenderCacheThemes(App app)
    {
        List<string> themes = [];

        if (!string.IsNullOrWhiteSpace(value: app.ConfigJson))
        {
            JObject config = JObject.Parse(json: app.ConfigJson);

            if (config["Themes"] is JObject configuredThemes)
            {
                themes.AddRange(
                    collection: configuredThemes.Properties()
                        .Select(selector: property => property.Name));
            }
        }

        themes.Add(
            item: string.IsNullOrWhiteSpace(value: app.DefaultTheme)
                ? "Default"
                : app.DefaultTheme);

        return
        [
            .. themes
                  .Where(predicate: theme => !string.IsNullOrWhiteSpace(value: theme))
                  .Select(selector: theme => theme.Trim())
                  .Distinct(comparer: StringComparer.OrdinalIgnoreCase)
          ];
    }

    private static string NormalizePageRenderCacheKey(string value) =>
        value.Trim()
            .ToLowerInvariant();

    internal static string CreatePageRenderCacheId(
        int appId,
        int pageId,
        string culture,
        string theme) =>
        $"{appId}_{pageId}_{culture}_{theme}";
}