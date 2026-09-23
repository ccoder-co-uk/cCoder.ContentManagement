// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Models;

internal sealed class PageRenderData
{
    public Page Page { get; init; }

    public Layout[] Layouts { get; init; }

    public Template[] Templates { get; init; }

    public Resource[] Resources { get; init; }

    public Component[] Components { get; init; }

    public Script[] Scripts { get; init; }

    public Page[] Pages { get; init; }
}