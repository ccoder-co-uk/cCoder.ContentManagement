// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Models;

public class PageRenderParams : ComponentRenderParams
{
    public Page Page { get; set; }

    public bool Edit { get; set; }
}