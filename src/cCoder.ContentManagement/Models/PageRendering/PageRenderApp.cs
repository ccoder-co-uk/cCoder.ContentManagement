// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Rendering.Models;

internal sealed class PageRenderApp
{
    public int Id { get; set; }

    public string Name { get; set; }
    public string Domain { get; set; }
    public string DefaultTheme { get; set; }
    public string DefaultCulture { get; set; }
    public object Config { get; set; }

    public IReadOnlyDictionary<string, PageRenderTemplate> TemplatesByName { get; set; }
    public IReadOnlyDictionary<int, PageRenderPage> PagesById { get; set; }

    internal PageRenderApp
()
    {
        this.Name = string.Empty;
        this.Domain = string.Empty;
        this.DefaultTheme = string.Empty;
        this.DefaultCulture = string.Empty;
        this.TemplatesByName = new Dictionary<string, PageRenderTemplate>(comparer: StringComparer.OrdinalIgnoreCase);
        this.PagesById = new Dictionary<int, PageRenderPage>();
    }
}