using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Models;

public class PageRenderParams : ComponentRenderParams
{
    public Page Page { get; }

    public bool Edit { get; }

    public PageRenderParams(Page page, string theme, App app, User user, string culture, bool edit = false)
        : base(theme, app, user, culture)
    {
        Page = page;
        Edit = edit;
    }
}
