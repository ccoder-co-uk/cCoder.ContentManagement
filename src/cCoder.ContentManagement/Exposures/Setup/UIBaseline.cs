using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Exposures.Setup;

public static partial class UIBaseline
{
    public static Package[] Packages => [
        Components,
        Pages,
        Resources,
        Layouts,
        Templates,
        Scripts,
        PageRoles
    ];
}
