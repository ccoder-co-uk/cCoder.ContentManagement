// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Rendering.Models;

internal sealed class PageRenderTemplate
{
    public string Name { get; set; }
    public string ResourceKey { get; set; }
    public string RawString { get; set; }

    internal PageRenderTemplate
()
    {
        this.Name = string.Empty;
        this.ResourceKey = string.Empty;
        this.RawString = string.Empty;
    }
}