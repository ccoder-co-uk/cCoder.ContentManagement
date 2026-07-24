// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Dependencies.Rendering;

internal sealed class PageRenderSession
{
    public PageRenderEngineRequest Request { get; init; }

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

    internal PageRenderSession()
    {
        Resources = Array.Empty<PageRenderResource>();
        ResourcesByLookup = new Dictionary<string, PageRenderResource>(comparer: StringComparer.OrdinalIgnoreCase);
        ComponentsByName = new Dictionary<string, PageRenderComponent>(comparer: StringComparer.OrdinalIgnoreCase);
        ScriptsByName = new Dictionary<string, PageRenderScript>(comparer: StringComparer.OrdinalIgnoreCase);
        MetadataResolver = static unusedKey => string.Empty;
        CommonResourcesByLookup = new Dictionary<string, PageRenderResource>(comparer: StringComparer.OrdinalIgnoreCase);
        CommonComponentsByName = new Dictionary<string, PageRenderComponent>(comparer: StringComparer.OrdinalIgnoreCase);
        CommonScriptsByName = new Dictionary<string, PageRenderScript>(comparer: StringComparer.OrdinalIgnoreCase);
    }
}