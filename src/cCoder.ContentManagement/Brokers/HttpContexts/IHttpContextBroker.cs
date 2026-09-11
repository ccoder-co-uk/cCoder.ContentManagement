// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Brokers.HttpContexts;

internal interface IHttpContextBroker
{
    HttpContext GetHttpContext();

    object GetRouteValue(string key);

    bool TryGetQueryValue(string key, out string value);

    string GetSessionValue(string key);

    string GetEncodedRequestUrl();
}