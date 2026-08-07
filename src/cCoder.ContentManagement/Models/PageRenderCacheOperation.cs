// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Models;

internal sealed class PageRenderCacheOperation
{
    public HttpPageRenderOperation RenderOperation { get; set; }

    public PageRenderCache Cache { get; set; }
}