using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Exposures;

namespace cCoder.ContentManagement.Services.Coordinations;

internal sealed partial class PageRenderCoordinationService
{
    private static void ValidateRequest(PageRenderRequest request, string parameterName)
    {
        if (request == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
    }

    private static void ValidateAppId(int appId, string parameterName)
    {
        if (appId < 1)
        {
            throw new ValidationException(parameterName + " must be greater than 0.");
        }
    }

    private static void ValidateTheme(string theme, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(theme))
        {
            throw new ValidationException(parameterName + " is required.");
        }
    }

    private static void ValidateHost(string host)
    {
        if (string.IsNullOrWhiteSpace(host))
        {
            throw new ValidationException("host is required.");
        }
    }

    private static void ValidateException(Exception exception, string parameterName)
    {
        if (exception == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
    }
}
