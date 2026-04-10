using System.ComponentModel.DataAnnotations;
using Culture = cCoder.Data.Models.CMS.Culture;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class CultureService
{
    private static void ValidateId(string id, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ValidationException(parameterName + " is required.");
        }
    }

    private static void ValidateCulture(Culture culture, string parameterName)
    {
        if ((object)culture == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
        if (string.IsNullOrWhiteSpace(culture.Id))
        {
            throw new ValidationException(parameterName + ".Id is required.");
        }
        if (string.IsNullOrWhiteSpace(culture.Name))
        {
            throw new ValidationException(parameterName + ".Name is required.");
        }
    }
}
