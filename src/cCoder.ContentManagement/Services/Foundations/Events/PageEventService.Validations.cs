using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal partial class PageEventService
{
    private static Page ValidatePage(Page page, string parameterName)
    {
        if (page == null)
            throw new ValidationException(parameterName + " is required.");

        return page;
    }
}
