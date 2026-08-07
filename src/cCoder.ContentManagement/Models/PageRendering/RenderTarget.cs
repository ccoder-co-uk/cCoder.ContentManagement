// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Models.PageRendering;

internal sealed class RenderTarget
{
    public RenderScope Scope { get; set; }

    public string ResourceKey { get; set; }

    public string HeaderMarkup { get; set; }

    public string BodyMarkup { get; set; }

    public object Model { get; set; }

    public bool AllowHeaderContentTags { get; set; }

    public bool AllowBodyContentTags { get; set; }
}