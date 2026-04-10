using System.ComponentModel.DataAnnotations;

namespace cCoder.ContentManagement.Services.Coordinations;

internal sealed partial class ComponentRenderCoordinationService
{
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
}
