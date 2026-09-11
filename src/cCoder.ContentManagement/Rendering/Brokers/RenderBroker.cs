// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Exposures;
using cCoder.ContentManagement.Models.PageRendering;

namespace cCoder.ContentManagement.Rendering.Brokers;

internal sealed class RenderBroker(
    IRenderSessionManager renderSessionManager)
        : IRenderBroker
{
    public RenderSession RenderRenderSession(RenderSession renderSession) =>
        renderSessionManager.RenderRenderSession(renderSession: renderSession);
}