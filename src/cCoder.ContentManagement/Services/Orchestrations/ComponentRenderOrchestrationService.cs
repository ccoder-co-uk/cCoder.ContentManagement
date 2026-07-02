using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models.Security;

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

    private static void ValidateAppId(int appId, string parameterName) =>
        ThrowIf(appId < 1, parameterName + " must be greater than 0.");

    private static void ValidateName(string name, string parameterName) =>
        ThrowIf(string.IsNullOrWhiteSpace(name), parameterName + " is required.");

    private static void ValidateTheme(string theme, string parameterName) =>
        ThrowIf(string.IsNullOrWhiteSpace(theme), parameterName + " is required.");

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
