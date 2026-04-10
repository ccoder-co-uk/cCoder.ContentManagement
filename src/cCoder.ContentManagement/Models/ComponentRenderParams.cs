namespace cCoder.ContentManagement.Models;

public class ComponentRenderParams : RenderParams
{
    public string Theme { get; }

    public ComponentRenderParams(string theme, cCoder.Data.Models.CMS.App app, cCoder.Data.Models.Security.User user, string culture)
        : base(app, user, culture)
    {
        Theme = theme ?? "Default";
    }
}
