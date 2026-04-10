using cCoder.ContentManagement.Models;
using Page = cCoder.Data.Models.CMS.Page;
using User = cCoder.Data.Models.Security.User;
using RenderResult = cCoder.ContentManagement.Models.RenderResult;

namespace cCoder.ContentManagement.Services.Processings;

public interface IPageRenderProcessingService
{
    RenderResult RenderPage(Page page, User user, Config config, string theme, string culture, bool edit = false);
}
