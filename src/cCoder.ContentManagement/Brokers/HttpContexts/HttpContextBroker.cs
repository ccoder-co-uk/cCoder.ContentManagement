// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Brokers.HttpContexts;

internal sealed class HttpContextBroker(HttpContext httpContext)
    : IHttpContextBroker
{
    public HttpContext GetHttpContext() =>
        httpContext;
}