// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Rendering.Models;

internal sealed class PageRenderEngineRequest
{
    public int AppId { get; set; }

    public string Path { get; set; }
    public string Theme { get; set; }
    public string Culture { get; set; }
    public bool Edit { get; set; }

    public string RequestUrl { get; set; }
    public Exception Exception { get; set; }

    internal PageRenderEngineRequest
    ()
    {
        this.Path = string.Empty;
        this.Theme = string.Empty;
        this.Culture = string.Empty;
        this.RequestUrl = string.Empty;
    }
}