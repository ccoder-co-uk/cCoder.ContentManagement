// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Dependencies.Rendering;

internal sealed class ResolvedPageRenderDefaults
{
    public App App { get; init; }

    public string Theme { get; init; }

    public string Culture { get; init; }
}