using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Processings;
using Page = cCoder.Data.Models.CMS.Page;
using User = cCoder.Data.Models.Security.User;
using RenderResult = cCoder.ContentManagement.Models.RenderResult;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal class PageRenderOrchestrationService(
    Config config,
    IPageRenderProcessingService pageRenderProcessingService) : IPageRenderOrchestrationService
{
    public RenderResult Render(Page page, User user, string theme, string culture, bool edit = false)
    {
        ValidatePage(page, "page");
        ValidateUser(user, "user");
        ValidateTheme(theme, "theme");

        return pageRenderProcessingService.RenderPage(page, user, config, theme, culture, edit);
    }

    private static void ValidatePage(Page page, string parameterName)
    {
        if (page == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
    }

    private static void ValidateUser(User user, string parameterName)
    {
        if (user == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
    }

    private static void ValidateTheme(string theme, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(theme))
        {
            throw new ValidationException(parameterName + " is required.");
        }
    }
}
