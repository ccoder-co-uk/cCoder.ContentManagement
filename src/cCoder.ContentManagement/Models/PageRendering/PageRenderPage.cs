// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Dependencies.Rendering;

internal sealed class PageRenderPage
{
    public int Id { get; set; }

    public int? ParentId { get; set; }

    public int AppId { get; set; }

    public int Order { get; set; }

    public bool ShowOnMenus { get; set; }

    public string Path { get; set; }
    public string Name { get; set; }
    public string ResourceKey { get; set; }
    public string LayoutName { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Keywords { get; set; }
    public IReadOnlyDictionary<string, PageRenderContent> ContentByName { get; set; }

    internal PageRenderPage
()
    {
        this.Path = string.Empty;
        this.Name = string.Empty;
        this.ResourceKey = string.Empty;
        this.LayoutName = string.Empty;
        this.Title = string.Empty;
        this.Description = string.Empty;
        this.Keywords = string.Empty;
        this.ContentByName = new Dictionary<string, PageRenderContent>(comparer: StringComparer.OrdinalIgnoreCase);
    }
}