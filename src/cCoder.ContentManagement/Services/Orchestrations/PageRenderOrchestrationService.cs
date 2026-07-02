using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

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

    private static void ValidatePage(Page page, string parameterName) =>
        ThrowIf(page == null, parameterName + " is required.");

    private static void ValidateUser(User user, string parameterName) =>
        ThrowIf(user == null, parameterName + " is required.");

    private static void ValidateTheme(string theme, string parameterName) =>
        ThrowIf(string.IsNullOrWhiteSpace(theme), parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
            throw new ValidationException(message);
    }
}
