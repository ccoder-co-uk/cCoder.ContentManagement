using cCoder.ContentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Processings;

public interface ITemplateRenderProcessingService
{
    string Render(int appId, string name, object model, User user, string culture, Config config, ILogger log = null);

    string RenderTemplate(Template template, object model, RenderParams renderParams, Config config, ILogger log = null);
}
