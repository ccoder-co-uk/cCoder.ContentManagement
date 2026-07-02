using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Exposures;

namespace cCoder.ContentManagement.Services.Coordinations;

internal sealed partial class PageRenderCoordinationService
{
    private static void ValidateRequest(PageRenderRequest request, string parameterName) =>
        ThrowIf(request == null, parameterName + " is required.");

    private static void ValidateAppId(int appId, string parameterName) =>
        ThrowIf(appId < 1, parameterName + " must be greater than 0.");

    private static void ValidateTheme(string theme, string parameterName) =>
        ThrowIf(string.IsNullOrWhiteSpace(theme), parameterName + " is required.");

    private static void ValidateHost(string host) =>
        ThrowIf(string.IsNullOrWhiteSpace(host), "host is required.");

    private static void ValidateException(Exception exception, string parameterName) =>
        ThrowIf(exception == null, parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
            throw new ValidationException(message);
    }
}
