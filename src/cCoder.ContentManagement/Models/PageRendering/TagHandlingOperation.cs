// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Models.PageRendering;

using cCoder.ContentManagement.Models.Rendering;

internal sealed class TagHandlingOperation
{
    public RenderSession Session { get; set; }

    public string ResourceKey { get; set; }

    public string Content { get; set; }

    public bool AllowContentTags { get; set; }

    public bool Editable { get; set; }

    public IReadOnlyCollection<MarkupReplacement> Replacements { get; set; }

    public ICollection<TagHandlingFragment> Fragments { get; set; }

    public object Value { get; set; }

    public bool Condition { get; set; }

    public IReadOnlyCollection<KeyValuePair<string, object>> JsonProperties { get; set; }

    public RuntimePropertyValue[] RuntimeProperties { get; set; }
}