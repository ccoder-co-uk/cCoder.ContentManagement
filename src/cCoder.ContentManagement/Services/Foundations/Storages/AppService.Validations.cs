using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class AppService
{
    private static void ValidateId(int id, string parameterName) =>
        ThrowIf(id < 1, parameterName + " must be greater than 0.");

    private static void ValidateApp(App app, string parameterName)
    {
        if (app == null)
            throw new ValidationException(parameterName + " is required.");

        if (string.IsNullOrWhiteSpace(app.Name))
            throw new ValidationException(parameterName + ".Name is required.");

        if (string.IsNullOrWhiteSpace(app.Domain))
            throw new ValidationException(parameterName + ".Domain is required.");
    }

    private static void ValidatePages(IEnumerable<Page> pages, string parameterName) =>
        ThrowIf(pages == null, parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
            throw new ValidationException(message);
    }
}
