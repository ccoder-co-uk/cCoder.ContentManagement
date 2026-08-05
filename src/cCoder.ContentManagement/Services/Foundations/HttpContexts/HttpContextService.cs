// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.HttpContexts;
using cCoder.ContentManagement.Models;
using Microsoft.AspNetCore.Http.Extensions;

namespace cCoder.ContentManagement.Services.Foundations.HttpContexts;

internal sealed partial class HttpContextService(
    IHttpContextBroker httpContextBroker) : IHttpContextService
{
    public HttpPageRenderContext GetPageRenderContext() =>
        TryCatch(operation: () =>
    {
        ValidateGetPageRenderContext(inputs: []);

        HttpContext context = httpContextBroker.GetHttpContext();
        HttpRequest request = context.Request;

        string path = request.RouteValues.TryGetValue(
                key: "path",
                value: out object routePath)
            ? routePath?.ToString()
            : request.Path.Value?.Trim(trimChar: '/');

        string culture = request.Query.TryGetValue(
                key: "culture",
                value: out Microsoft.Extensions.Primitives.StringValues cultureValue)
            ? cultureValue.ToString()
            : context.Session.GetString(key: "culture");

        string theme = request.Query.TryGetValue(
                key: "theme",
                value: out Microsoft.Extensions.Primitives.StringValues themeValue)
            ? themeValue.ToString()
            : context.Session.GetString(key: "theme");

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
            Theme = theme,
            Nonce = context.Items[
                    ContentSecurityPolicyNonceContract.HttpContextItemKey]
                ?.ToString()
                ?? string.Empty,
            RequestUrl = request.GetEncodedUrl(),
            Edit = request.Query.TryGetValue(
                    key: "edit",
                    value: out Microsoft.Extensions.Primitives.StringValues editValue)
                && bool.TryParse(
                    value: editValue.ToString(),
                    result: out bool edit)
                && edit
        };
    });
}