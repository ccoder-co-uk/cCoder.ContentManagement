using System.ComponentModel.DataAnnotations;
using CommonObject = cCoder.Data.Models.CommonObject;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class CommonObjectService
{
    private static void ValidateId(int id, string parameterName)
    {
        if (id < 1)
        {
            throw new ValidationException(parameterName + " must be greater than 0.");
        }
    }

    private static void ValidateCommonObject(CommonObject commonObject, string parameterName)
    {
        if (commonObject == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
        if (string.IsNullOrWhiteSpace(commonObject.Name))
        {
            throw new ValidationException(parameterName + ".Name is required.");
        }
        if (string.IsNullOrWhiteSpace(commonObject.Type))
        {
            throw new ValidationException(parameterName + ".Type is required.");
        }
    }
}

