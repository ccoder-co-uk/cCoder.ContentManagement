namespace cCoder.ContentManagement.Models;

public abstract class RenderParams
{
    public cCoder.Data.Models.CMS.App App { get; }

    public cCoder.Data.Models.Security.User User { get; }

    public string Culture { get; set; }

    protected RenderParams(cCoder.Data.Models.CMS.App app, cCoder.Data.Models.Security.User user)
        : this(app, user, "")
    {
    }

    protected RenderParams(cCoder.Data.Models.CMS.App app, cCoder.Data.Models.Security.User user, string culture)
    {
        App = app;
        User = user;
        Culture = culture;
    }
}
