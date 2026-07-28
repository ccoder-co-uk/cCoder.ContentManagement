// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.PageRendering;

namespace cCoder.ContentManagement.Rendering.Services.Orchestrations;

internal interface IPageRenderExecutionOrchestrationService
{
    PageRenderSession RenderPageRenderSession(PageRenderSession session);
}