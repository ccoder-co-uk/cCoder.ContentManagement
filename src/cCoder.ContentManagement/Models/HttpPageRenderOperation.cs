// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Models;

internal sealed class HttpPageRenderOperation
{
    public HttpPageRenderContext Context { get; set; }

    public PageRenderResponse Response { get; set; }
}