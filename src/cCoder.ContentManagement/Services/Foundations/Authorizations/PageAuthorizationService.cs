// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using cCoder.ContentManagement.Brokers.Authorizations;
using cCoder.ContentManagement.Models;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Authorizations;

internal sealed partial class PageAuthorizationService(
    IPageAuthorizationBroker pageAuthorizationBroker)
        : IPageAuthorizationService
{
    public ValueTask<HttpPageRenderContext> AuthorizeHttpPageRenderContextAsync(
        HttpPageRenderContext httpPageRenderContext) =>
        TryCatch(operation: async () =>
    {
        ValidateAuthorizeHttpPageRenderContextAsync(
            inputs: [httpPageRenderContext]);

        ValidatePageRenderContext(
            pageRenderContext: httpPageRenderContext,
            parameterName: "pageRenderContext");

        PageAuthorizationData authorizationData = await pageAuthorizationBroker
            .GetAuthorizedPageAsync(
                domain: httpPageRenderContext.Domain,
                path: httpPageRenderContext.Path,
                culture: httpPageRenderContext.Culture,
                theme: httpPageRenderContext.Theme);

        PageAuthorizationResult authorization = authorizationData.Result;
        httpPageRenderContext.User = authorizationData.User;

        if (authorization?.PageId is not null)
        {
            ApplyAuthorization(
                pageRenderContext: httpPageRenderContext,
                authorization: authorization);

            httpPageRenderContext.PrefetchedPageRenderCache = ResolveCache(
                candidates: authorization.CacheCandidates,
                culture: httpPageRenderContext.Culture);

            httpPageRenderContext.PageRenderCacheLookupCompleted =
                authorizationData.CacheLookupCompleted;

            if (httpPageRenderContext.Edit)
            {
                httpPageRenderContext.Edit = await pageAuthorizationBroker
                    .CanUpdatePageAsync(
                        appId: authorization.AppId,
                        pageId: authorization.PageId.Value);
            }

            return httpPageRenderContext;
        }

        authorizationData = await pageAuthorizationBroker
            .GetPageIgnoringFiltersAsync(
                domain: httpPageRenderContext.Domain,
                path: httpPageRenderContext.Path);

        authorization = authorizationData.Result;
        httpPageRenderContext.User = authorizationData.User;

        if (authorization?.PageId is not null)
        {
            ApplyAuthorization(
                pageRenderContext: httpPageRenderContext,
                authorization: authorization);

            httpPageRenderContext.AccessDenied = true;

            return httpPageRenderContext;
        }

        if (authorization is not null)
        {
            ApplyAuthorization(
                pageRenderContext: httpPageRenderContext,
                authorization: authorization);
        }

        return httpPageRenderContext;

    }, isValueTask: true);

    private static void ApplyAuthorization(
        HttpPageRenderContext pageRenderContext,
        PageAuthorizationResult authorization)
    {
        pageRenderContext.PageId = authorization.PageId;
        pageRenderContext.Layout = authorization.Layout;
        pageRenderContext.AppId = authorization.AppId;
        pageRenderContext.TenantId = authorization.TenantId;
        pageRenderContext.Domain = authorization.Domain;
        pageRenderContext.AppConfigJson = authorization.AppConfigJson;
        pageRenderContext.AppDefaultTheme = authorization.DefaultTheme;

        if (string.IsNullOrWhiteSpace(value: pageRenderContext.Culture))
        {
            pageRenderContext.Culture = authorization.DefaultCulture;
        }

        if (string.IsNullOrWhiteSpace(value: pageRenderContext.Theme))
        {
            pageRenderContext.Theme = authorization.DefaultTheme;
        }
    }

    private static PageRenderCache ResolveCache(
        PageRenderCache[] candidates,
        string culture)
    {
        if (candidates is null)
        {
            return null;
        }

        foreach (string fallbackCulture in ResolveCultureFallbacks(culture: culture))
        {
            PageRenderCache match = candidates.FirstOrDefault(
                predicate: cache => string.Equals(
                    a: cache.Culture,
                    b: fallbackCulture,
                    comparisonType: StringComparison.OrdinalIgnoreCase));

            if (match is not null)
            {
                return match;
            }
        }

        return null;
    }

    private static string[] ResolveCultureFallbacks(string culture)
    {
        List<string> cultures = [];
        string current = (culture ?? string.Empty).Trim();

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

        return
        [
            .. cultures.Distinct(
                comparer: StringComparer.OrdinalIgnoreCase)
        ];
    }
}