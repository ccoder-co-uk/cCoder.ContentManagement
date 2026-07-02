using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class CommonObjectService
{
    private static void ValidateId(int id, string parameterName) =>
        ThrowIf(id < 1, parameterName + " must be greater than 0.");

    private static void ValidateCommonObject(CommonObject commonObject, string parameterName)
    {
        if (commonObject == null)
            throw new ValidationException(parameterName + " is required.");

        if (string.IsNullOrWhiteSpace(commonObject.Name))
            throw new ValidationException(parameterName + ".Name is required.");

        if (string.IsNullOrWhiteSpace(commonObject.Type))
            throw new ValidationException(parameterName + ".Type is required.");
    }

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
            throw new ValidationException(message);
    }
}
