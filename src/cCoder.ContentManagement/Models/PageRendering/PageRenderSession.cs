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

    public IReadOnlyList<PageRenderResource> Resources { get; set; } = Array.Empty<PageRenderResource>();

    public IReadOnlyDictionary<string, PageRenderResource> ResourcesByLookup { get; set; } =
        new Dictionary<string, PageRenderResource>(StringComparer.OrdinalIgnoreCase);

    public IDictionary<string, PageRenderComponent> ComponentsByName { get; set; } =
        new Dictionary<string, PageRenderComponent>(StringComparer.OrdinalIgnoreCase);

    public IDictionary<string, PageRenderScript> ScriptsByName { get; set; } =
        new Dictionary<string, PageRenderScript>(StringComparer.OrdinalIgnoreCase);

    public Func<string, string> MetadataResolver { get; set; } = static _ => string.Empty;

    public IReadOnlyDictionary<string, PageRenderResource> CommonResourcesByLookup { get; set; } =
        new Dictionary<string, PageRenderResource>(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyDictionary<string, PageRenderComponent> CommonComponentsByName { get; set; } =
        new Dictionary<string, PageRenderComponent>(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyDictionary<string, PageRenderScript> CommonScriptsByName { get; set; } =
        new Dictionary<string, PageRenderScript>(StringComparer.OrdinalIgnoreCase);
}
