// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Models;

public sealed class PageRenderCacheMiss
{
    public int AppId { get; set; }

    public int PageId { get; set; }

    public string Culture { get; set; }

    public string Theme { get; set; }
}