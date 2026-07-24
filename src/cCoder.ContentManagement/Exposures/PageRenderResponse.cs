// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Exposures;

public sealed class PageRenderResponse
{
    public required App App { get; init; }

    public required RenderResult Page { get; init; }

    public required string Theme { get; init; }

    public required string Culture { get; init; }

    public required bool Edit { get; init; }
}