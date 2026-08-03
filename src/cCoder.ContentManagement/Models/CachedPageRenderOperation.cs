// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Models;

internal sealed class CachedPageRenderOperation
{
    public int AppId { get; set; }

    public int PageId { get; set; }

    public Page Page { get; set; }

    public string Culture { get; set; }

    public string Theme { get; set; }

    public User User { get; set; }

    public RenderResult RenderResult { get; set; }
}