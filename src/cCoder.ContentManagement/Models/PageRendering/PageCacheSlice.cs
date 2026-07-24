// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Dependencies.Rendering;

internal sealed class PageCacheSlice
{
    public IReadOnlyDictionary<string, PageRenderResource> CommonResourcesByLookup { get; init; }
    public IReadOnlyDictionary<string, PageRenderComponent> CommonComponentsByName { get; init; }
    public IReadOnlyDictionary<string, PageRenderScript> CommonScriptsByName { get; init; }

    internal PageCacheSlice
()
    {
        this.CommonResourcesByLookup = new Dictionary<string, PageRenderResource>(comparer: StringComparer.OrdinalIgnoreCase);
        this.CommonComponentsByName = new Dictionary<string, PageRenderComponent>(comparer: StringComparer.OrdinalIgnoreCase);
        this.CommonScriptsByName = new Dictionary<string, PageRenderScript>(comparer: StringComparer.OrdinalIgnoreCase);
    }
}