using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class PageService
{
    private static void ValidateId(int id, string parameterName) =>
        ThrowIf(id < 1, parameterName + " must be greater than 0.");

    private static void ValidatePage(Page page, string parameterName)
    {
        if (page == null)
            throw new ValidationException(parameterName + " is required.");

        if (page.AppId < 1)
            throw new ValidationException(parameterName + ".AppId must be greater than 0.");

        if (string.IsNullOrWhiteSpace(page.Name))
            throw new ValidationException(parameterName + ".Name is required.");
    }

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
            throw new ValidationException(message);
    }
}
