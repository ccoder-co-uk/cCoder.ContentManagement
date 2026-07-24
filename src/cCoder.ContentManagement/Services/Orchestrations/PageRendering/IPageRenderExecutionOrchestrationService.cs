// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies.Rendering;

namespace cCoder.ContentManagement.Rendering.Services.Orchestrations;

internal interface IPageRenderExecutionOrchestrationService
{
    PageRenderResult RenderPageRenderSessionPageRenderResult(PageRenderSession session);
}