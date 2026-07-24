// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Dependencies.Rendering;

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

    internal PageRenderResult
()
    {
        this.Theme = string.Empty;
        this.Culture = string.Empty;
        this.Path = string.Empty;
        this.Layout = string.Empty;
        this.Title = string.Empty;
        this.Description = string.Empty;
        this.Keywords = string.Empty;
        this.HeaderHtml = string.Empty;
        this.BodyHtml = string.Empty;
        this.StatusCode = 200;
    }
}