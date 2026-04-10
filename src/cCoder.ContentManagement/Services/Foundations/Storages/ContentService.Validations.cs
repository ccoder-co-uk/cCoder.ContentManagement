using System.ComponentModel.DataAnnotations;
using Content = cCoder.Data.Models.CMS.Content;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class ContentService
{
    private static void ValidateId(int id, string parameterName)
    {
        if (id < 1)
        {
            throw new ValidationException(parameterName + " must be greater than 0.");
        }
    }

    private static void ValidateContent(Content content, string parameterName)
    {
        if (content == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
        if (content.PageId < 1)
        {
            throw new ValidationException(parameterName + ".PageId must be greater than 0.");
        }
        if (content.CultureId == null)
        {
            throw new ValidationException(parameterName + ".CultureId is required.");
        }
        if (string.IsNullOrWhiteSpace(content.Name))
        {
            throw new ValidationException(parameterName + ".Name is required.");
        }
    }
}
