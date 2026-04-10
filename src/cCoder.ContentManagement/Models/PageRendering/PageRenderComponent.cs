namespace cCoder.ContentManagement.Rendering.Models;

internal sealed class PageRenderComponent
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string ResourceKey { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string Script { get; set; } = string.Empty;
}
