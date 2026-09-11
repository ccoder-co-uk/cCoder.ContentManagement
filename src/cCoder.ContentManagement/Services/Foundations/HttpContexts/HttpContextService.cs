// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.HttpContexts;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Foundations.HttpContexts;

internal sealed partial class HttpContextService(
    IHttpContextBroker httpContextBroker) : IHttpContextService
{
    public HttpPageRenderContext GetPageRenderContext() =>
        TryCatch(operation: () =>
    {

        HttpContext context = httpContextBroker.GetHttpContext();
        HttpRequest request = context.Request;

        string path = httpContextBroker.GetRouteValue(key: "path")?.ToString()
            ?? request.Path.Value?.Trim(trimChar: '/');

        bool hasCultureQuery = httpContextBroker.TryGetQueryValue(
            key: "culture",
            value: out string cultureValue);

        bool cultureWasExplicitlyRequested = hasCultureQuery
            || string.Equals(
                a: httpContextBroker.GetSessionValue(key: "cultureexplicit"),
                b: bool.TrueString,
                comparisonType: StringComparison.OrdinalIgnoreCase);

        string culture = hasCultureQuery
            ? cultureValue.ToString()
            : httpContextBroker.GetSessionValue(key: "culture");

        string theme = httpContextBroker.TryGetQueryValue(
                key: "theme",
                value: out string themeValue)
            ? themeValue
            : httpContextBroker.GetSessionValue(key: "theme");

        return new HttpPageRenderContext
        {
            Domain = request.Host.Host
                .Replace(
                    oldValue: "www.",
                    newValue: string.Empty,
                    comparisonType: StringComparison.OrdinalIgnoreCase)
                .ToLowerInvariant(),
            Path = path ?? string.Empty,
            Culture = culture,
            CultureWasExplicitlyRequested = cultureWasExplicitlyRequested,
            Theme = theme,
            Nonce = context.Items[
                    ContentSecurityPolicyNonceContract.HttpContextItemKey]
                ?.ToString()
                ?? string.Empty,
            RequestUrl = httpContextBroker.GetEncodedRequestUrl(),
            Edit = httpContextBroker.TryGetQueryValue(
                    key: "edit",
                    value: out string editValue)
                && bool.TryParse(
                    value: editValue,
                    result: out bool edit)
                && edit
        };
    });
}