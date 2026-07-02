using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class PageInfoService
{
    private static void ValidateId(int id, string parameterName) =>
        ThrowIf(id < 1, parameterName + " must be greater than 0.");

    private static void ValidatePageInfo(PageInfo pageInfo, string parameterName)
    {
        if (pageInfo == null)
            throw new ValidationException(parameterName + " is required.");

        if (pageInfo.PageId < 1)
            throw new ValidationException(parameterName + ".PageId must be greater than 0.");

        if (pageInfo.CultureId == null)
            throw new ValidationException(parameterName + ".CultureId is required.");
    }

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
            throw new ValidationException(message);
    }
}
