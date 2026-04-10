using System.ComponentModel.DataAnnotations;

namespace cCoder.ContentManagement.Services.Foundations;

internal partial class RenderFileContentService
{
    private static int ValidateAppId(int appId, string parameterName)
    {
        if (appId < 1)
        {
            throw new ValidationException(parameterName + " must be greater than 0.");
        }

        return appId;
    }

    private static string ValidatePath(string path, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ValidationException(parameterName + " is required.");
        }

        return path;
    }
}
