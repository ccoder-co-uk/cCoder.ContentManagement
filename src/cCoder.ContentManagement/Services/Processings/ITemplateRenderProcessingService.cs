using Config = cCoder.ContentManagement.Models.Config;
using RenderParams = cCoder.ContentManagement.Models.RenderParams;
using Template = cCoder.Data.Models.CMS.Template;
using User = cCoder.Data.Models.Security.User;

namespace cCoder.ContentManagement.Services.Processings;

public interface ITemplateRenderProcessingService
{
    string Render(int appId, string name, object model, User user, string culture, Config config, ILogger log = null);

    string RenderTemplate(Template template, object model, RenderParams renderParams, Config config, ILogger log = null);
}
