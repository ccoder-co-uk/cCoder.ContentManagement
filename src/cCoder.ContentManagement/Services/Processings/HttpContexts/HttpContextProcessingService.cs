// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Foundations.HttpContexts;

namespace cCoder.ContentManagement.Services.Processings.HttpContexts;

internal sealed partial class HttpContextProcessingService(
    IHttpContextService httpContextService)
        : IHttpContextProcessingService
{
    public HttpPageRenderContext GetPageRenderContext() =>
        TryCatch<HttpPageRenderContext>(operation: () =>
            httpContextService.GetPageRenderContext());
}