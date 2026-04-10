using Page = cCoder.Data.Models.CMS.Page;
using User = cCoder.Data.Models.Security.User;
using RenderResult = cCoder.ContentManagement.Models.RenderResult;

namespace cCoder.ContentManagement.Services.Orchestrations;

public interface IPageRenderOrchestrationService
{
    RenderResult Render(Page page, User user, string theme, string culture, bool edit = false);
}
