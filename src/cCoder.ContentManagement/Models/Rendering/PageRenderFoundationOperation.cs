// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.PageRendering;

namespace cCoder.ContentManagement.Models.Rendering;

internal sealed class PageRenderFoundationOperation
{
    public object Value { get; set; }

    public string Json { get; set; }

    public RenderSession RenderSession { get; set; }
}