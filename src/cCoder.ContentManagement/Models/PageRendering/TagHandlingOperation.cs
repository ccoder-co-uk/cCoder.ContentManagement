// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Models.PageRendering;

internal sealed class TagHandlingOperation
{
    public RenderSession Session { get; set; }

    public string ResourceKey { get; set; }

    public string Content { get; set; }

    public bool AllowContentTags { get; set; }

    public bool Editable { get; set; }

    public IReadOnlyCollection<MarkupReplacement> Replacements { get; set; }

    public ICollection<TagHandlingFragment> Fragments { get; set; }
}