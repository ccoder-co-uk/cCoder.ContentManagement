// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Models;

public sealed class PageRenderResponse
{
    public App App { get; init; }

    public RenderResult Page { get; init; }

    public string Theme { get; init; }

    public string Culture { get; init; }

    public bool Edit { get; init; }
}