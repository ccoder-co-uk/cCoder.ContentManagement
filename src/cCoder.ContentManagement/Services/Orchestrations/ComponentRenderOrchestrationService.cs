using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Processings;
using User = cCoder.Data.Models.Security.User;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal sealed class ComponentRenderOrchestrationService(
    IComponentRenderProcessingService componentRenderProcessingService) : IComponentRenderOrchestrationService
{
    public string Render(int appId, string name, User user, string culture, string theme)
    {
        ValidateAppId(appId, "appId");
        ValidateName(name, "name");
        ValidateUser(user, "user");
        ValidateTheme(theme, "theme");

        return componentRenderProcessingService.Render(appId, name, user, culture, theme);
    }

    private static void ValidateAppId(int appId, string parameterName)
    {
        if (appId < 1)
        {
            throw new ValidationException(parameterName + " must be greater than 0.");
        }
    }

    private static void ValidateName(string name, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(name))
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

    private static User ValidateUser(User user, string parameterName)
    {
        if (user == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }

        return user;
    }
}
