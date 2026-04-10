namespace cCoder.ContentManagement.Rendering.Models;

internal sealed class PageCacheSlice
{
    public IReadOnlyDictionary<string, PageRenderResource> CommonResourcesByLookup { get; init; } =
        new Dictionary<string, PageRenderResource>(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyDictionary<string, PageRenderComponent> CommonComponentsByName { get; init; } =
        new Dictionary<string, PageRenderComponent>(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyDictionary<string, PageRenderScript> CommonScriptsByName { get; init; } =
        new Dictionary<string, PageRenderScript>(StringComparer.OrdinalIgnoreCase);
}
