using cCoder.ContentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Orchestrations;

public interface IPageRenderOrchestrationService
{
    RenderResult Render(Page page, User user, string theme, string culture, bool edit = false);
}
