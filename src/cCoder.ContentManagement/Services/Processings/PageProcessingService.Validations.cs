using System.ComponentModel.DataAnnotations;
using Page = cCoder.Data.Models.CMS.Page;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class PageProcessingService
{
    private static void ValidateId(int id, string parameterName)
    {
        if (id < 1)
        {
            throw new ValidationException(parameterName + " must be greater than 0.");
        }
    }

    private static void ValidateAppId(int appId, string parameterName)
    {
        if (appId < 1)
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
    }

    private static void ValidatePages(IEnumerable<Page> pages, string parameterName)
    {
        if (pages == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
    }
}
