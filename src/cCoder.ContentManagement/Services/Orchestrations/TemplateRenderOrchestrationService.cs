using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Processings;
using cCoder.ContentManagement.Models;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal sealed class TemplateRenderOrchestrationService(
    ITemplateRenderProcessingService templateRenderProcessingService,
    Config config,
    ILogger<TemplateRenderOrchestrationService> log) : ITemplateRenderOrchestrationService
{
    public string Render(int appId, string name, string culture, dynamic model, User user)
    {
        ValidateAppId(appId, "appId");
        ValidateTemplateName(name, "name");
        ValidateUser(user, "user");

        return templateRenderProcessingService.Render(appId, name, model, user, culture, config, log);
    }

    private static void ValidateAppId(int appId, string parameterName) =>
        ThrowIf(appId < 1, parameterName + " must be greater than 0.");

    private static void ValidateTemplateName(string name, string parameterName) =>
        ThrowIf(string.IsNullOrWhiteSpace(name), parameterName + " is required.");

    private static User ValidateUser(User user, string parameterName)
    {
        if (user == null)
            throw new ValidationException(parameterName + " is required.");

        return user;
    }

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
            throw new ValidationException(message);
    }
}
