// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Rendering.Models;

internal sealed class PageRenderComponent
{
    public int Id { get; set; }

    public string Name { get; set; }
    public string ResourceKey { get; set; }
    public string Content { get; set; }
    public string Script { get; set; }

    internal PageRenderComponent
()
    {
        this.Name = string.Empty;
        this.ResourceKey = string.Empty;
        this.Content = string.Empty;
        this.Script = string.Empty;
    }
}