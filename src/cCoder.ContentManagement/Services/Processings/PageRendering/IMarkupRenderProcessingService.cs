// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.PageRendering;

namespace cCoder.ContentManagement.Rendering.Services.Processings;

internal interface IMarkupRenderProcessingService
{
    RenderSession RenderRenderSession(RenderSession session);
}