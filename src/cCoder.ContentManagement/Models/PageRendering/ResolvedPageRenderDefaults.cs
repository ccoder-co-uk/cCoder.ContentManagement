// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Rendering.Models;

internal sealed class ResolvedPageRenderDefaults
{
    public required App App { get; init; }

    public required string Theme { get; init; }

    public required string Culture { get; init; }
}