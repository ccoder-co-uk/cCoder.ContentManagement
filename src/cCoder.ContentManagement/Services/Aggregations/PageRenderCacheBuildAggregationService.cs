// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security.Cryptography;
using System.Text;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace cCoder.ContentManagement.Services.Aggregations;

internal sealed partial class PageRenderCacheBuildAggregationService(
    IPageOrchestrationService pageOrchestrationService,
    IAppCultureOrchestrationService appCultureOrchestrationService,
    IPageRenderOrchestrationService pageRenderOrchestrationService,
    IPageRenderCacheOrchestrationService pageRenderCacheOrchestrationService)
        : IPageRenderCacheBuildAggregationService
{
    public ValueTask BuildPageAsync(int pageId) =>
        TryCatch(operation: async () =>
    {
        ValidateBuildPageAsync(inputs: [pageId]);
        ValidatePageId(pageId: pageId, parameterName: "pageId");

        Page page = await pageOrchestrationService.GetPageByIdForRenderAsync(
            pageId: pageId);

        if (page?.App is null)
        {
            return;
        }

        App app = page.App;

        PageRenderCache[] replacements = BuildPageRenderCaches(
            app: app,
            page: page);

        await pageRenderCacheOrchestrationService
            .ReplacePageRenderCachesFromEventAsync(
                appId: app.Id,
                pageIds: [page.Id],
                replacements: replacements);
    }, isValueTask: true);

    private PageRenderCache[] BuildPageRenderCaches(
        App app,
        Page page)
    {
        string[] cultures = ResolveCultures(app: app);

        string[] themes = ResolveThemes(app: app);

        List<PageRenderCache> replacements = new(
            capacity: cultures.Length * themes.Length);

        foreach (string culture in cultures)
        {
            foreach (string theme in themes)
            {
                PageRenderOperation operation = new()
                {
                    OperationType = PageRenderOperationType.RenderResult,
                    SourcePage = page,
                    Theme = theme,
                    Culture = culture,
                    Edit = false,
                    CacheTemplate = true
                };

                RenderResult result = pageRenderOrchestrationService
                    .ProcessPageRenderOperation(operation: operation)
                    .Page;

                string fingerprintSource = JsonConvert.SerializeObject(
                    value: result,
                    formatting: Formatting.None);

                replacements.Add(item: new PageRenderCache
                {
                    Id = CreateId(
                        appId: app.Id,
                        pageId: page.Id,
                        culture: culture,
                        theme: theme),
                    AppId = app.Id,
                    PageId = page.Id,
                    Culture = culture,
                    Theme = theme,
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
                            source: Encoding.UTF8.GetBytes(
                                s: fingerprintSource))),
                    RenderedOn = DateTimeOffset.UtcNow
                });
            }
        }

        return [.. replacements];
    }

    private string[] ResolveCultures(App app)
    {
        return
        [
            .. appCultureOrchestrationService.GetAllAppCulture(
                    ignoreFilters: true)
                .Where(predicate: appCulture => appCulture.AppId == app.Id)
                .Select(selector: appCulture => appCulture.CultureId)
                .Where(predicate: culture => culture.Length > 0)
                .AsEnumerable()
                .Select(selector: culture => Normalize(value: culture))
                .Distinct(comparer: StringComparer.OrdinalIgnoreCase)
                .Prepend(element: string.Empty)
        ];
    }

    private static string[] ResolveThemes(App app)
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
                .Where(predicate: theme =>
                    !string.IsNullOrWhiteSpace(value: theme))
                .Select(selector: theme => Normalize(value: theme))
                .Distinct(comparer: StringComparer.OrdinalIgnoreCase)
        ];
    }

    private static string Normalize(string value) =>
        value.Trim()
            .ToLowerInvariant();

    private static string CreateId(
        int appId,
        int pageId,
        string culture,
        string theme) =>
        $"{appId}_{pageId}_{Normalize(value: culture)}_{Normalize(value: theme)}";
}