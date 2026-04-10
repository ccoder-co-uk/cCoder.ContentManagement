using System.ComponentModel.DataAnnotations;
using App = cCoder.Data.Models.CMS.App;
using Page = cCoder.Data.Models.CMS.Page;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class AppService
{
    private static void ValidateId(int id, string parameterName)
    {
        if (id < 1)
        {
            throw new ValidationException(parameterName + " must be greater than 0.");
        }
    }

    private static void ValidateApp(App app, string parameterName)
    {
        if (app == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
        if (string.IsNullOrWhiteSpace(app.Name))
        {
            throw new ValidationException(parameterName + ".Name is required.");
        }
        if (string.IsNullOrWhiteSpace(app.Domain))
        {
            throw new ValidationException(parameterName + ".Domain is required.");
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
