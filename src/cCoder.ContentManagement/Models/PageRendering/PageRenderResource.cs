// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Rendering.Models;

internal sealed class PageRenderResource
{
    public string Key { get; set; }
    public string Culture { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string ShortDisplayName { get; set; }
    public string Description { get; set; }

    internal PageRenderResource
()
    {
        this.Key = string.Empty;
        this.Culture = string.Empty;
        this.Name = string.Empty;
        this.DisplayName = string.Empty;
        this.ShortDisplayName = string.Empty;
        this.Description = string.Empty;
    }
}