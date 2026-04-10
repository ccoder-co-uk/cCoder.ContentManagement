namespace cCoder.ContentManagement.Models;

public class RenderResult
{
    public int AppId { get; set; }

    public int PageId { get; set; }

    public int? ParentId { get; set; }

    public string UserId { get; set; }

    public bool ShowOnMenus { get; set; }

    public bool Edit { get; set; }

    public string Culture { get; set; }

    public string Theme { get; set; }

    public string Path { get; set; }

    public string Layout { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public string Keywords { get; set; }

    public string HeaderHtml { get; set; }

    public string BodyHtml { get; set; }

    public int StatusCode { get; set; } = 200;

    public dynamic KeyInfo()
    {
        return new { AppId, PageId, ParentId };
    }
}
