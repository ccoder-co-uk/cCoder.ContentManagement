using System.ComponentModel.DataAnnotations;
using AppCulture = cCoder.Data.Models.CMS.AppCulture;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class AppCultureService
{
    private static void ValidateAppId(int appId, string parameterName)
    {
        if (appId < 1)
        {
            throw new ValidationException(parameterName + " must be greater than 0.");
        }
    }

    private static void ValidateCultureId(string cultureId, string parameterName)
    {
        if (cultureId == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
    }

    private static void ValidateAppCulture(AppCulture appCulture, string parameterName)
    {
        if (appCulture == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
        if (appCulture.AppId < 1)
        {
            throw new ValidationException(parameterName + ".AppId must be greater than 0.");
        }
        if (appCulture.CultureId == null)
        {
            throw new ValidationException(parameterName + ".CultureId is required.");
        }
    }
}
