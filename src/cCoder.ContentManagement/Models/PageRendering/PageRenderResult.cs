namespace cCoder.ContentManagement.Rendering.Models;

internal sealed class PageRenderResult
{
    public int AppId { get; set; }

    public int PageId { get; set; }

    public int? ParentId { get; set; }

    public string Theme { get; set; } = string.Empty;

    public string Culture { get; set; } = string.Empty;

    public bool Edit { get; set; }

    public string Path { get; set; } = string.Empty;

    public string Layout { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Keywords { get; set; } = string.Empty;

    public string HeaderHtml { get; set; } = string.Empty;

    public string BodyHtml { get; set; } = string.Empty;

    public int StatusCode { get; set; } = 200;
}
