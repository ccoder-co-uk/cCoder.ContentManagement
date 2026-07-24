// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        ValidatePage(page: page, parameterName: "page");
        ValidateUser(user: user, parameterName: "user");
        ValidateTheme(theme: theme, parameterName: "theme");

        return pageRenderProcessingService.RenderPage(page: page, user: user, config: config, theme: theme, culture: culture, edit: edit);
    }

    private static void ValidatePage(Page page, string parameterName) =>
        ThrowIf(condition: page == null, message: parameterName + " is required.");

    private static void ValidateUser(User user, string parameterName) =>
        ThrowIf(condition: user == null, message: parameterName + " is required.");

    private static void ValidateTheme(string theme, string parameterName) =>
        ThrowIf(condition: string.IsNullOrWhiteSpace(value: theme), message: parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }
}