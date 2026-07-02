using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class PageProcessingService
{
    private static void ValidateId(int id, string parameterName) =>
        ThrowIf(id < 1, parameterName + " must be greater than 0.");

    private static void ValidateAppId(int appId, string parameterName) =>
        ThrowIf(appId < 1, parameterName + " must be greater than 0.");

    private static void ValidatePage(Page page, string parameterName) =>
        ThrowIf(page == null, parameterName + " is required.");

    private static void ValidatePages(IEnumerable<Page> pages, string parameterName) =>
        ThrowIf(pages == null, parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
            throw new ValidationException(message);
    }
}
