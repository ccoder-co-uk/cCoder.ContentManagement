// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Exposures;

public interface IPageRenderer
{
    ValueTask<PageRenderResponse> RenderAsync();
}