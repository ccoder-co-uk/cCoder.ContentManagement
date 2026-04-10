using System.ComponentModel.DataAnnotations;

namespace cCoder.ContentManagement.Services.Foundations.Exports;

internal partial class PackageExportService
{
    private static int ValidateAppId(int appId, string parameterName)
    {
        if (appId < 1)
        {
            throw new ValidationException(parameterName + " must be greater than 0.");
        }

        return appId;
    }
}
