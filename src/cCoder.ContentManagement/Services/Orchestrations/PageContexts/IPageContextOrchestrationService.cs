// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Orchestrations.PageContexts;

internal interface IPageContextOrchestrationService
{
    ValueTask<HttpPageRenderContext> ResolvePageRenderContextAsync();
}