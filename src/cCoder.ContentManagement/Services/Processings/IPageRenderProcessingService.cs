using cCoder.ContentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Processings;

public interface IPageRenderProcessingService
{
    RenderResult RenderPage(Page page, User user, Config config, string theme, string culture, bool edit = false);
}
