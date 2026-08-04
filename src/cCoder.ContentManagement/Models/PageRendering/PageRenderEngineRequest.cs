// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Models.PageRendering;

internal sealed class PageRenderEngineRequest
{
    public int AppId { get; set; }

    public string Path { get; set; }
    public string Theme { get; set; }
    public string Culture { get; set; }
    public bool Edit { get; set; }
    public bool HeaderOnly { get; set; }

    public bool CacheTemplate { get; set; }

    public string RequestUrl { get; set; }
    public Exception Exception { get; set; }

}