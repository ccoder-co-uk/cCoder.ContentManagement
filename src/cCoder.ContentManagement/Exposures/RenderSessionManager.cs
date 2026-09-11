// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.PageRendering;
using cCoder.ContentManagement.Rendering.Services.Orchestrations;

namespace cCoder.ContentManagement.Exposures;

internal sealed class RenderSessionManager(
    IRenderOrchestrationService renderOrchestrationService)
        : IRenderSessionManager
{
    public RenderSession RenderRenderSession(RenderSession renderSession) =>
        renderOrchestrationService.RenderRenderSession(
            session: renderSession);
}