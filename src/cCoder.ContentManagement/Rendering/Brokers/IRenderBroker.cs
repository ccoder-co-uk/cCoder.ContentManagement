// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.PageRendering;

namespace cCoder.ContentManagement.Rendering.Brokers;

internal interface IRenderBroker
{
    RenderSession RenderRenderSession(RenderSession renderSession);
}