// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Rendering.Models;

internal sealed class PageRenderSession
{
    public required PageRenderEngineRequest Request { get; init; }

    public Config Config { get; set; }

    public PageRenderApp App { get; set; }

    public PageRenderPage Page { get; set; }

    public PageRenderUser User { get; set; }

    public PageRenderLayout Layout { get; set; }

    public IReadOnlyList<PageRenderResource> Resources { get; set; }
    public IReadOnlyDictionary<string, PageRenderResource> ResourcesByLookup { get; set; }
    public IDictionary<string, PageRenderComponent> ComponentsByName { get; set; }
    public IDictionary<string, PageRenderScript> ScriptsByName { get; set; }
    public Func<string, string> MetadataResolver { get; set; }
    public IReadOnlyDictionary<string, PageRenderResource> CommonResourcesByLookup { get; set; }
    public IReadOnlyDictionary<string, PageRenderComponent> CommonComponentsByName { get; set; }
    public IReadOnlyDictionary<string, PageRenderScript> CommonScriptsByName { get; set; }

    internal PageRenderSession
()
    {
        this.Resources = Array.Empty<PageRenderResource>();
        this.ResourcesByLookup = new Dictionary<string, PageRenderResource>(comparer: StringComparer.OrdinalIgnoreCase);
        this.ComponentsByName = new Dictionary<string, PageRenderComponent>(comparer: StringComparer.OrdinalIgnoreCase);
        this.ScriptsByName = new Dictionary<string, PageRenderScript>(comparer: StringComparer.OrdinalIgnoreCase);
        this.MetadataResolver = static unusedKey => string.Empty;
        this.CommonResourcesByLookup = new Dictionary<string, PageRenderResource>(comparer: StringComparer.OrdinalIgnoreCase);
        this.CommonComponentsByName = new Dictionary<string, PageRenderComponent>(comparer: StringComparer.OrdinalIgnoreCase);
        this.CommonScriptsByName = new Dictionary<string, PageRenderScript>(comparer: StringComparer.OrdinalIgnoreCase);
    }
}