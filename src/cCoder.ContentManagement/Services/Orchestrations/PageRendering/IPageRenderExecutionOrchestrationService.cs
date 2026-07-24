// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Rendering.Models;

namespace cCoder.ContentManagement.Rendering.Services.Orchestrations;

internal interface IPageRenderExecutionOrchestrationService
{
    PageRenderResult RenderPageRenderSessionPageRenderResult(PageRenderSession session);
}