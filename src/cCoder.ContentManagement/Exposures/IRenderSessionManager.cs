// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.PageRendering;

namespace cCoder.ContentManagement.Exposures;

internal interface IRenderSessionManager
{
    RenderSession RenderRenderSession(RenderSession renderSession);
}