// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Models;

internal sealed class PageRenderCacheImportState
{
    private readonly AsyncLocal<bool> active = new();

    public bool Active
    {
        get => active.Value;
        set => active.Value = value;
    }
}