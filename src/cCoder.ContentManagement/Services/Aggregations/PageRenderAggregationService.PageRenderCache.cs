// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security.Cryptography;
using System.Text;
using cCoder.ContentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace cCoder.ContentManagement.Services.Aggregations;

internal sealed partial class PageRenderAggregationService
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

    public ValueTask DeleteAppPageRenderCacheAsync(int appId) =>
        TryCatch(operation: () =>
        {
            ValidatePageRenderCacheByAppIdOnDelete(inputs: [appId]);

            return pageRenderCacheOrchestrationService.DeleteAppPageRenderCachesAsync(
                appId: appId);
        }, isValueTask: true);

    public ValueTask DeleteAppPageRenderCacheFromEventAsync(int appId) =>
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

    public ValueTask DeletePagePageRenderCacheAsync(int pageId) =>
        TryCatch(operation: () =>
        {
            ValidatePageRenderCacheByPageIdOnDelete(inputs: [pageId]);

            return pageRenderCacheOrchestrationService.DeletePagePageRenderCachesAsync(
                pageId: pageId);
        }, isValueTask: true);

    public ValueTask DeletePagePageRenderCacheFromEventAsync(int pageId) =>
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

    public ValueTask<PageRenderOperation> RebuildAllAppsPageRenderOperationAsync(
        PageRenderOperation operation) =>
        TryCatch<PageRenderOperation>(operation: async () =>
        {
            ValidateAllPageRenderCachesOnRebuild(inputs: [operation]);

            operation.PageRenderCaches = await ExecuteRebuildAllAppsPageRenderCacheAsync(
                fromEvent: false);

            return operation;
        }, isValueTask: true);

    public ValueTask<PageRenderOperation> RebuildCommonObjectPageRenderOperationAsync(
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

    public ValueTask<PageRenderOperation> RebuildAppPageRenderOperationAsync(
        PageRenderOperation operation) =>
        TryCatch<PageRenderOperation>(operation: async () =>
        {
            ValidatePageRenderCachesByAppIdOnRebuild(inputs: [operation]);

            if (pageRenderCacheImportState.Active && operation.RebuildCache)
            {
                return operation;
            }

            if (operation.RebuildCache &&
                !pageRenderCacheOrchestrationService
                    .GetAllPageRenderCaches()
                    .Any(predicate: cache => cache.AppId == operation.AppId))
            {
                return operation;
            }

            return await ExecuteRebuildAppPageRenderOperationAsync(operation: operation);
        }, isValueTask: true);

    private async ValueTask<PageRenderOperation> ExecuteRebuildAppPageRenderOperationAsync(
        PageRenderOperation operation)
    {
        App app = appOrchestrationService.GetAllApp(ignoreFilters: true)
            .FirstOrDefault(predicate: app => app.Id == operation.AppId)
                ?? throw new KeyNotFoundException(
                    message: $"App {operation.AppId} was not found.");

        Page[] pages =
        [
            .. pageOrchestrationService.GetAllPage(ignoreFilters: true)
                    .Where(predicate: page => page.AppId == operation.AppId)
        ];

        List<PageRenderCache> rebuilt = [];

        foreach (Page page in pages)
        {
            rebuilt.AddRange(
                collection: BuildPageRenderCaches(
                    app: app,
                    page: page));
        }

        int[] pageIds = pages.Select(selector: page => page.Id)
            .ToArray();

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

    public ValueTask<PageRenderOperation> RebuildPagePageRenderOperationAsync(
        PageRenderOperation operation) =>
        TryCatch<PageRenderOperation>(operation: async () =>
        {
            ValidatePageRenderCachesByPageIdOnRebuild(inputs: [operation]);

            if (pageRenderCacheImportState.Active && operation.RebuildCache)
            {
                return operation;
            }

            if (operation.RebuildCache &&
                !pageRenderCacheOrchestrationService
                    .GetAllPageRenderCaches()
                    .Any(predicate: cache => cache.PageId == operation.PageId))
            {
                return operation;
            }

            Page page = pageOrchestrationService.GetAllPage(ignoreFilters: true)
                .FirstOrDefault(predicate: page => page.Id == operation.PageId)
                    ?? throw new KeyNotFoundException(
                        message: $"Page {operation.PageId} was not found.");

            App app = appOrchestrationService.GetAllApp(ignoreFilters: true)
                .FirstOrDefault(predicate: app => app.Id == page.AppId)
                    ?? throw new KeyNotFoundException(
                        message: $"App {page.AppId} was not found.");

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
                PageRenderOperation operation = RenderPageRenderOperation(
                    operation: new PageRenderOperation
                    {
                        OperationType = PageRenderOperationType.RenderResult,
                        AppId = app.Id,
                        Path = page.Path ?? string.Empty,
                        Culture = culture,
                        Theme = theme,
                        RebuildCache = true
                    });

                RenderResult result = operation.Page as RenderResult
                    ?? throw new InvalidOperationException(
                        message: $"Page {page.Id} did not produce a render result.");

                if (result.Edit)
                {
                    throw new InvalidOperationException(
                        message: "Edit-mode results cannot be cached.");
                }

                result.HeaderHtml = string.Empty;

                string value = JsonConvert.SerializeObject(
                    value: result,
                    formatting: Formatting.None);

                replacements.Add(item: new PageRenderCache
                {
                    AppId = app.Id,
                    PageId = page.Id,
                    Culture = NormalizePageRenderCacheKey(value: culture),
                    Theme = NormalizePageRenderCacheKey(value: theme),
                    Value = value,
                    HeaderValue = string.Empty,
                    SourceFingerprint = Convert.ToHexString(
                        inArray: SHA256.HashData(
                            source: Encoding.UTF8.GetBytes(s: value))),
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
            .. appCultureOrchestrationService.GetAllAppCulture(
                    ignoreFilters: true)
                .Where(predicate: appCulture => appCulture.AppId == app.Id)
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
}