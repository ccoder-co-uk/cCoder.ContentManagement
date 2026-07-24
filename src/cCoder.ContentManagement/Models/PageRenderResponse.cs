// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Models;

public class PageRenderResponse
{
    public App App { get; set; }

    public RenderResult Page { get; set; }

    public string Theme { get; set; }

    public string Culture { get; set; }

    public bool Edit { get; set; }
}