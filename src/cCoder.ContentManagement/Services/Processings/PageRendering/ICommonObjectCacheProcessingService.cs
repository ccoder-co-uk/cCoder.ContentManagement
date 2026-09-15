// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.PageRendering;

namespace cCoder.ContentManagement.Rendering.Services.Processings;

internal interface ICommonObjectCacheProcessingService
{
    void Refresh();

    RenderSession PrepareRenderSession(RenderSession session);
}
