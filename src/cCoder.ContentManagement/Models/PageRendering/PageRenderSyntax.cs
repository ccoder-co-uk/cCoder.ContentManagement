using System.Text.RegularExpressions;

namespace cCoder.ContentManagement.Rendering.Models;

internal sealed class PageRenderSyntax
{
    public required Regex ContentRegex { get; init; }

    public required Regex ResourceDisplayNameRegex { get; init; }

    public required Regex ResourceShortDisplayNameRegex { get; init; }

    public required Regex ResourceDescriptionRegex { get; init; }

    public required Regex NavRegex { get; init; }

    public required Regex NavExpandedRegex { get; init; }

    public required Regex DmsRegex { get; init; }

    public required Regex ExecuteRegex { get; init; }

    public required Regex ComponentRegex { get; init; }

    public required Regex ScriptRegex { get; init; }

    public required Regex MetaRegex { get; init; }

    public required Regex CultureLinkRegex { get; init; }
}
