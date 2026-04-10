using System.ComponentModel.DataAnnotations;
using Page = cCoder.Data.Models.CMS.Page;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class PageService
{
    private static void ValidateId(int id, string parameterName)
    {
        if (id < 1)
        {
            throw new ValidationException(parameterName + " must be greater than 0.");
        }
    }

    private static void ValidatePage(Page page, string parameterName)
    {
        if (page == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
        if (page.AppId < 1)
        {
            throw new ValidationException(parameterName + ".AppId must be greater than 0.");
        }
        if (string.IsNullOrWhiteSpace(page.Name))
        {
            throw new ValidationException(parameterName + ".Name is required.");
        }
    }
}
