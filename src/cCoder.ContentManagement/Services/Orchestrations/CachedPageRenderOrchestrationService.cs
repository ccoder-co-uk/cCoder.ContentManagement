// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Models.PageRendering;
using cCoder.ContentManagement.Services.Aggregations;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Newtonsoft.Json;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal sealed partial class CachedPageRenderOrchestrationService(
    IPageRenderCacheQueryProcessingService pageRenderCacheProcessingService,
    IPageRenderCacheMissEventProcessingService eventProcessingService)
        : ICachedPageRenderOrchestrationService
{
    public ValueTask<CachedPageRenderOperation> RenderCachedPageRenderOperationAsync(
        CachedPageRenderOperation operation) =>
        TryCatch<CachedPageRenderOperation>(operation: async () =>
    {
        ValidateCachedPageRenderOperationOnRender(inputs: [operation]);

        ValidateCachedPageRenderOperation(
            operation: operation,
            parameterName: "operation");

        string normalizedCulture = operation.Culture.Trim()
            .ToLowerInvariant();

        string normalizedTheme = operation.Theme.Trim()
            .ToLowerInvariant();

        string cacheId = PageRenderAggregationService.CreatePageRenderCacheId(
            appId: operation.AppId,
            pageId: operation.PageId,
            culture: normalizedCulture,
            theme: normalizedTheme);

        PageRenderCache cached = pageRenderCacheProcessingService
            .GetPageRenderCache(pageRenderCacheId: cacheId);

        if (cached is null)
        {
            await eventProcessingService.RaisePageRenderCacheMissEventAsync(
                cacheMiss: new PageRenderCacheMiss
                {
                    AppId = operation.AppId,
                    PageId = operation.PageId,
                    Culture = normalizedCulture,
                    Theme = normalizedTheme
                });

            cached = pageRenderCacheProcessingService.GetPageRenderCache(
                pageRenderCacheId: cacheId);

            if (cached is null)
            {
                return operation;
            }
        }

        RenderResult result = new()
        {
            AppId = cached.AppId,
            PageId = cached.PageId,
            ParentId = cached.ParentId,
            UserId = null,
            ShowOnMenus = cached.ShowOnMenus,
            Edit = false,
            Culture = operation.Culture,
            Theme = operation.Theme,
            Path = cached.Path,
            Layout = operation.Page.Layout,
            Title = cached.Title,
            Description = cached.Description,
            Keywords = cached.Keywords,
            HeaderHtml = cached.Header,
            BodyHtml = cached.Body,
            StatusCode = StatusCodes.Status200OK
        };

        HydrateRuntimeValues(
            result: result,
            page: operation.Page,
            user: operation.User,
            culture: operation.Culture);

        operation.RenderResult = result;
        return operation;

    }, isValueTask: true);

    private static void HydrateRuntimeValues(
        RenderResult result,
        Page page,
        User user,
        string culture)
    {
        bool isGuest = string.IsNullOrWhiteSpace(value: user?.Id)
            || string.Equals(
                a: user.Id,
                b: "Guest",
                comparisonType: StringComparison.OrdinalIgnoreCase);

        string resolvedCulture = string.IsNullOrWhiteSpace(value: culture)
            ? user?.DefaultCultureId ?? page.App?.DefaultCultureId ?? string.Empty
            : culture;

        string serializedUser = JsonConvert.SerializeObject(value: new
        {
            Id = isGuest ? "Guest" : user.Id,
            DefaultCultureId = string.IsNullOrWhiteSpace(value: user?.DefaultCultureId)
                ? resolvedCulture
                : user.DefaultCultureId,
            DisplayName = isGuest ? "Guest" : user.DisplayName,
            Email = user?.Email ?? string.Empty
        });

        string loginResourceName = isGuest ? "Login" : "Logout";

        string loginLabel = ResolveRuntimeResourceLabel(
            page: page,
            name: loginResourceName,
            culture: resolvedCulture);

        string loginLink = isGuest
            ? $"<a href='/Login'>{loginLabel}</a>"
            : $"<a name='logout' href=''>{loginLabel}</a>";

        string currentDate = DateTimeOffset.UtcNow.ToString(format: "dd MMM yyyy");

        result.BodyHtml = HydrateRuntimeValues(
            value: result.BodyHtml,
            serializedUser: serializedUser,
            displayName: isGuest ? "Guest" : user.DisplayName,
            loginLink: loginLink,
            currentDate: currentDate);

        result.HeaderHtml = HydrateRuntimeValues(
            value: result.HeaderHtml,
            serializedUser: serializedUser,
            displayName: isGuest ? "Guest" : user.DisplayName,
            loginLink: loginLink,
            currentDate: currentDate);
    }

    private static string HydrateRuntimeValues(
        string value,
        string serializedUser,
        string displayName,
        string loginLink,
        string currentDate) =>
        value.Replace(
                oldValue: PageRenderRuntimeTokens.User,
                newValue: serializedUser,
                comparisonType: StringComparison.Ordinal)
            .Replace(
                oldValue: PageRenderRuntimeTokens.DisplayName,
                newValue: displayName,
                comparisonType: StringComparison.Ordinal)
            .Replace(
                oldValue: PageRenderRuntimeTokens.LoginLink,
                newValue: loginLink,
                comparisonType: StringComparison.Ordinal)
            .Replace(
                oldValue: PageRenderRuntimeTokens.Date,
                newValue: currentDate,
                comparisonType: StringComparison.Ordinal);

    private static string ResolveRuntimeResourceLabel(
        Page page,
        string name,
        string culture)
    {
        string pageKey = string.IsNullOrWhiteSpace(value: page.ResourceKey)
            ? "Default"
            : page.ResourceKey;

        Resource resource = page.App?.Resources?
            .Where(predicate: candidate => string.Equals(
                a: candidate.Name,
                b: name,
                comparisonType: StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(keySelector: candidate => string.Equals(
                a: candidate.Culture,
                b: culture,
                comparisonType: StringComparison.OrdinalIgnoreCase))
            .ThenByDescending(keySelector: candidate => string.IsNullOrEmpty(
                value: candidate.Culture))
            .ThenByDescending(keySelector: candidate => string.Equals(
                a: candidate.Key,
                b: pageKey,
                comparisonType: StringComparison.OrdinalIgnoreCase))
            .ThenByDescending(keySelector: candidate => string.Equals(
                a: candidate.Key,
                b: "Default",
                comparisonType: StringComparison.OrdinalIgnoreCase))
            .FirstOrDefault();

        return resource?.DisplayName ?? name;
    }
}