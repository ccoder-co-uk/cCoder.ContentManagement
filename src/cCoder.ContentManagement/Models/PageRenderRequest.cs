// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Models;

public sealed class PageRenderRequest
{
    public string Host { get; set; }
    public string Path { get; set; }
    public string Theme { get; set; }
    public string Culture { get; set; }
    public bool Edit { get; set; }

    public string RequestUrl { get; set; }
    public Exception Exception { get; set; }

    public PageRenderRequest()
    {
        Host = string.Empty;
        Path = string.Empty;
        Theme = string.Empty;
        Culture = string.Empty;
        RequestUrl = string.Empty;
    }
}