namespace cCoder.ContentManagement.Exposures;

public sealed class PageRenderRequest
{
    public string Host { get; set; } = string.Empty;

    public string Path { get; set; } = string.Empty;

    public string Theme { get; set; } = string.Empty;

    public string Culture { get; set; } = string.Empty;

    public bool Edit { get; set; }

    public string RequestUrl { get; set; } = string.Empty;

    public Exception Exception { get; set; }
}
