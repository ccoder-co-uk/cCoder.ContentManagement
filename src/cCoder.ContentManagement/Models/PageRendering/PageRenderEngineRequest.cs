namespace cCoder.ContentManagement.Rendering.Models;

internal sealed class PageRenderEngineRequest
{
    public int AppId { get; set; }

    public string Path { get; set; } = string.Empty;

    public string Theme { get; set; } = string.Empty;

    public string Culture { get; set; } = string.Empty;

    public bool Edit { get; set; }

    public string RequestUrl { get; set; } = string.Empty;

    public Exception Exception { get; set; }
}
