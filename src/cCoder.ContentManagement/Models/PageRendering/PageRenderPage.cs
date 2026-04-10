namespace cCoder.ContentManagement.Rendering.Models;

internal sealed class PageRenderPage
{
    public int Id { get; set; }

    public int? ParentId { get; set; }

    public int AppId { get; set; }

    public int Order { get; set; }

    public bool ShowOnMenus { get; set; }

    public string Path { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string ResourceKey { get; set; } = string.Empty;

    public string LayoutName { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Keywords { get; set; } = string.Empty;

    public IReadOnlyDictionary<string, PageRenderContent> ContentByName { get; set; } =
        new Dictionary<string, PageRenderContent>(StringComparer.OrdinalIgnoreCase);
}
