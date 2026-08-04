// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Models.PageRendering;

internal sealed class PageRenderResult
{
    public int AppId { get; set; }

    public int PageId { get; set; }

    public int? ParentId { get; set; }

    public string Theme { get; set; }
    public string Culture { get; set; }
    public bool Edit { get; set; }

    public string Path { get; set; }
    public string Layout { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Keywords { get; set; }
    public string HeaderHtml { get; set; }
    public string BodyHtml { get; set; }
    public int StatusCode { get; set; }
}