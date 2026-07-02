using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class AppCultureService
{
    private static void ValidateAppId(int appId, string parameterName) =>
        ThrowIf(appId < 1, parameterName + " must be greater than 0.");

    private static void ValidateCultureId(string cultureId, string parameterName) =>
        ThrowIf(cultureId == null, parameterName + " is required.");

    private static void ValidateAppCulture(AppCulture appCulture, string parameterName)
    {
        if (appCulture == null)
            throw new ValidationException(parameterName + " is required.");

        if (appCulture.AppId < 1)
            throw new ValidationException(parameterName + ".AppId must be greater than 0.");

        if (appCulture.CultureId == null)
            throw new ValidationException(parameterName + ".CultureId is required.");
    }

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
            throw new ValidationException(message);
    }
}
