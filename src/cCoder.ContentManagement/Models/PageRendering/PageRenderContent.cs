// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Models.PageRendering;

internal sealed class PageRenderContent
{
    public int Id { get; set; }

    public string Name { get; set; }
    public string Html { get; set; }

    internal PageRenderContent
()
    {
        this.Name = string.Empty;
        this.Html = string.Empty;
    }
}