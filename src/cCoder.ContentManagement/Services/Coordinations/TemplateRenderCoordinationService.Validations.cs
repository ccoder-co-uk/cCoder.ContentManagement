using System.ComponentModel.DataAnnotations;

namespace cCoder.ContentManagement.Services.Coordinations;

internal sealed partial class TemplateRenderCoordinationService
{
    private static void ValidateAppId(int appId, string parameterName) =>
        ThrowIf(appId < 1, parameterName + " must be greater than 0.");

    private static void ValidateName(string name, string parameterName) =>
        ThrowIf(string.IsNullOrWhiteSpace(name), parameterName + " is required.");

    private static void ValidateModel(object model, string parameterName) =>
        ThrowIf(model == null, parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
            throw new ValidationException(message);
    }
}
