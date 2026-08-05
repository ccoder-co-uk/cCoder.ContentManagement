// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Models.PageRendering;

internal sealed class TagHandlingOperation
{
    public PageRenderSession Session { get; set; }

    public string ResourceKey { get; set; }

    public string Content { get; set; }

    public bool AllowContentTags { get; set; }

    public IReadOnlyCollection<ReplacementDependency> Replacements { get; set; }

    public ICollection<TagHandlingFragment> Fragments { get; set; }
}