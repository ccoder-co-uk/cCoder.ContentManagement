namespace cCoder.ContentManagement.Rendering.Models;

internal sealed class PageRenderApp
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Domain { get; set; } = string.Empty;

    public string DefaultTheme { get; set; } = string.Empty;

    public string DefaultCulture { get; set; } = string.Empty;

    public object Config { get; set; }

    public IReadOnlyDictionary<string, PageRenderTemplate> TemplatesByName { get; set; } =
        new Dictionary<string, PageRenderTemplate>(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyDictionary<int, PageRenderPage> PagesById { get; set; } =
        new Dictionary<int, PageRenderPage>();
}
