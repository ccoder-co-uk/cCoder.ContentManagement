// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections.Generic;
namespace cCoder.ContentManagement.Models.PageRendering;

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
}