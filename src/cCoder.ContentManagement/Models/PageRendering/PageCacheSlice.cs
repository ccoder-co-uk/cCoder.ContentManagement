// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Models.PageRendering;

internal sealed class PageCacheSlice
{
    public IReadOnlyDictionary<string, PageRenderResource> CommonResourcesByLookup { get; init; }
    public IReadOnlyDictionary<string, PageRenderComponent> CommonComponentsByName { get; init; }
    public IReadOnlyDictionary<string, PageRenderScript> CommonScriptsByName { get; init; }
    public IReadOnlyDictionary<string, PageRenderStyle> CommonStylesByName { get; init; }
}