namespace cCoder.ContentManagement.Models;

public class PageRenderParams : ComponentRenderParams
{
    public cCoder.Data.Models.CMS.Page Page { get; }

    public bool Edit { get; }

    public PageRenderParams(cCoder.Data.Models.CMS.Page page, string theme, cCoder.Data.Models.CMS.App app, cCoder.Data.Models.Security.User user, string culture, bool edit = false)
        : base(theme, app, user, culture)
    {
        Page = page;
        Edit = edit;
    }
}
