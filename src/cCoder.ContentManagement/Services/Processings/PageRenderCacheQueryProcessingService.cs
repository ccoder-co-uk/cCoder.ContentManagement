// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal sealed partial class PageRenderCacheQueryProcessingService(
    IPageRenderCacheService pageRenderCacheService)
        : IPageRenderCacheQueryProcessingService
{
    public IQueryable<PageRenderCache> GetAllPageRenderCaches() =>
        TryCatch<IQueryable<PageRenderCache>>(operation: () =>
        {

            return pageRenderCacheService.GetAllPageRenderCaches();
        });

    public PageRenderCache GetPageRenderCache(string pageRenderCacheId) =>
        TryCatch<PageRenderCache>(operation: () =>
        {
            ValidatePageRenderCacheOnGet(inputs: [pageRenderCacheId]);
            ArgumentException.ThrowIfNullOrWhiteSpace(argument: pageRenderCacheId);

            return pageRenderCacheService.GetPageRenderCache(
                pageRenderCacheId: pageRenderCacheId);
        });

    public PageRenderCache GetPageRenderCache(
        int pageId,
        string culture,
        string theme) =>
        TryCatch<PageRenderCache>(operation: () =>
        {
            ValidatePageRenderCacheOnGet(
                inputs: [pageId, culture, theme]);

            string[] cultures = ResolveCultureFallbacks(culture: culture);

            PageRenderCache[] matches =
            [
                .. pageRenderCacheService.GetAllPageRenderCaches()
                .Where(predicate: cache =>
                    cache.PageId == pageId
                    && cultures.Contains(value: cache.Culture)
                    && cache.Theme == theme)
            ];

            return cultures
                .Select(selector: fallbackCulture => matches
                    .SingleOrDefault(predicate: cache =>
                        cache.Culture == fallbackCulture))
                .FirstOrDefault(predicate: cache => cache is not null);
        });

    private static string[] ResolveCultureFallbacks(string culture)
    {
        List<string> cultures = [];
        string current = culture ?? string.Empty;

        while (!string.IsNullOrWhiteSpace(value: current))
        {
            cultures.Add(item: current);

            int separatorIndex = current.LastIndexOf(
                value: "-",
                comparisonType: StringComparison.Ordinal);

            current = separatorIndex < 0
                ? string.Empty
                : current[..separatorIndex];
        }

        cultures.Add(item: string.Empty);
        return [.. cultures.Distinct(comparer: StringComparer.OrdinalIgnoreCase)];
    }
}