// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Models.PageRendering;

internal sealed class PageRenderLayout
{
    public string Name { get; set; }
    public string HeaderHtml { get; set; }
    public string BodyHtml { get; set; }

    internal PageRenderLayout
()
    {
        this.Name = string.Empty;
        this.HeaderHtml = string.Empty;
        this.BodyHtml = string.Empty;
    }
}