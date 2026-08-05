// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Foundations.HttpContexts;

internal interface IHttpContextService
{
    HttpPageRenderContext GetPageRenderContext();
}