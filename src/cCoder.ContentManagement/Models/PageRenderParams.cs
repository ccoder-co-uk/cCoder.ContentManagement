// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Models;

public class PageRenderParams : ComponentRenderParams
{
    public Page Page { get; set; }

    public bool Edit { get; set; }
}