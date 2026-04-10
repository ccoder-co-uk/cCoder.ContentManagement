using System.ComponentModel.DataAnnotations;

namespace cCoder.ContentManagement.Services.Coordinations;

internal sealed partial class TemplateRenderCoordinationService
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

    private static void ValidateModel(object model, string parameterName)
    {
        if (model == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
    }
}
