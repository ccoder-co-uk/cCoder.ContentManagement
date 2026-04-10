using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Processings;
using Config = cCoder.ContentManagement.Models.Config;
using User = cCoder.Data.Models.Security.User;

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

    private static void ValidateAppId(int appId, string parameterName)
    {
        if (appId < 1)
        {
            throw new ValidationException(parameterName + " must be greater than 0.");
        }
    }

    private static void ValidateTemplateName(string name, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ValidationException(parameterName + " is required.");
        }
    }

    private static User ValidateUser(User user, string parameterName)
    {
        if (user == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }

        return user;
    }
}
