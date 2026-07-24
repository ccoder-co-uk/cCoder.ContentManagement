// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Rendering.Models;

internal sealed class PageCacheSlice
{
    public IReadOnlyDictionary<string, PageRenderResource> CommonResourcesByLookup { get; init; } =
        new Dictionary<string, PageRenderResource>(comparer: StringComparer.OrdinalIgnoreCase);

    public IReadOnlyDictionary<string, PageRenderComponent> CommonComponentsByName { get; init; } =
        new Dictionary<string, PageRenderComponent>(comparer: StringComparer.OrdinalIgnoreCase);

    public IReadOnlyDictionary<string, PageRenderScript> CommonScriptsByName { get; init; } =
        new Dictionary<string, PageRenderScript>(comparer: StringComparer.OrdinalIgnoreCase);
}