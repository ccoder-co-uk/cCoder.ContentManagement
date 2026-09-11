// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Brokers.HttpContexts;

using Microsoft.AspNetCore.Http.Extensions;

internal sealed class HttpContextBroker(HttpContext httpContext)
    : IHttpContextBroker
{
    public HttpContext GetHttpContext() =>
        httpContext;

    public object GetRouteValue(string key) =>
        httpContext.Request.RouteValues[key];

    public bool TryGetQueryValue(string key, out string value)
    {
        bool hasValue = httpContext.Request.Query.TryGetValue(
            key: key,
            value: out Microsoft.Extensions.Primitives.StringValues queryValue);

        value = queryValue.ToString();
        return hasValue;
    }

    public string GetSessionValue(string key) =>
        httpContext.Session.GetString(key: key);

    public string GetEncodedRequestUrl() =>
        httpContext.Request.GetEncodedUrl();

    public string GetRequestPath() =>
        httpContext?.Request.Path.Value ?? string.Empty;

    public string GetRequestHost() =>
        httpContext?.Request.Host.Host ?? string.Empty;
}