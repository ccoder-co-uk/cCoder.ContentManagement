// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Models.PageRendering;

internal sealed class PageRenderScript
{
    public string Name { get; set; }
    public string Content { get; set; }

    internal PageRenderScript
()
    {
        this.Name = string.Empty;
        this.Content = string.Empty;
    }
}