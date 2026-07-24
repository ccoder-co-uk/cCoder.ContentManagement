// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text.RegularExpressions;

namespace cCoder.ContentManagement.Rendering.Models;

internal sealed class PageRenderSyntax
{
    public Regex ContentRegex { get; init; }

    public Regex ResourceDisplayNameRegex { get; init; }

    public Regex ResourceShortDisplayNameRegex { get; init; }

    public Regex ResourceDescriptionRegex { get; init; }

    public Regex NavRegex { get; init; }

    public Regex NavExpandedRegex { get; init; }

    public Regex DmsRegex { get; init; }

    public Regex ExecuteRegex { get; init; }

    public Regex ComponentRegex { get; init; }

    public Regex ScriptRegex { get; init; }

    public Regex MetaRegex { get; init; }

    public Regex CultureLinkRegex { get; init; }
}