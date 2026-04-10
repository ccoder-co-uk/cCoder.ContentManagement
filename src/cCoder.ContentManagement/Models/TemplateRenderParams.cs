namespace cCoder.ContentManagement.Models;

public class TemplateRenderParams : RenderParams
{
    public TemplateRenderParams(cCoder.Data.Models.CMS.App app, cCoder.Data.Models.Security.User user, string culture)
        : base(app, user, culture)
    {
    }
}
