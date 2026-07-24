// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Exposures;

public sealed class PageRenderRequest
{
    public string Host { get; set; }
    public string Path { get; set; }
    public string Theme { get; set; }
    public string Culture { get; set; }
    public bool Edit { get; set; }

    public string RequestUrl { get; set; }
    public Exception Exception { get; set; }

    public PageRenderRequest
    ()
    {
        this.Host = string.Empty;
        this.Path = string.Empty;
        this.Theme = string.Empty;
        this.Culture = string.Empty;
        this.RequestUrl = string.Empty;
    }
}