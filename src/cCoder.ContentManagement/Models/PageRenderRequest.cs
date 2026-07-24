// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Models;

public sealed class PageRenderRequest : PageRenderOperation
{
    public PageRenderRequest()
    {
        Host = string.Empty;
        Path = string.Empty;
        Theme = string.Empty;
        Culture = string.Empty;
        RequestUrl = string.Empty;
    }
}