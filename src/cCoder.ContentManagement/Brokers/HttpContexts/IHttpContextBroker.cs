// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Brokers.HttpContexts;

internal interface IHttpContextBroker
{
    HttpContext GetHttpContext();
}