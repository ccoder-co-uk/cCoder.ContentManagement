using App = cCoder.Data.Models.CMS.App;

namespace cCoder.ContentManagement.Rendering.Models;

internal sealed class ResolvedPageRenderDefaults
{
    public required App App { get; init; }

    public required string Theme { get; init; }

    public required string Culture { get; init; }
}
