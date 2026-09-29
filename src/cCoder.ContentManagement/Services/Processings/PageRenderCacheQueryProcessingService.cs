// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
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

            List<PageRenderCache> matches = [];

            foreach (PageRenderCache cache in pageRenderCacheService
                .GetAllPageRenderCaches()
                .Where(predicate: cache =>
                    cache.PageId == pageId
                    && cache.Theme == theme))
            {
                matches.Add(item: cache);
            }

            foreach (string fallbackCulture in cultures)
            {
                foreach (PageRenderCache match in matches)
                {
                    if (match.Culture == fallbackCulture)
                    {
                        return match;
                    }
                }
            }

            return null;
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
        List<string> distinctCultures = [];

        foreach (string fallbackCulture in cultures)
        {
            bool exists = false;

            foreach (string distinctCulture in distinctCultures)
            {
                if (string.Equals(
                    a: distinctCulture,
                    b: fallbackCulture,
                    comparisonType: StringComparison.OrdinalIgnoreCase))
                {
                    exists = true;
                    break;
                }
            }

            if (!exists)
            {
                distinctCultures.Add(item: fallbackCulture);
            }
        }

        return [.. distinctCultures];
    }
}