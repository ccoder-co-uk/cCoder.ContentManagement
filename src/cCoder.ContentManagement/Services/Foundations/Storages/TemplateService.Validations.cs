using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class TemplateService
{
    private static void ValidateId(int id, string parameterName) =>
        ThrowIf(id < 1, parameterName + " must be greater than 0.");

    private static void ValidateTemplate(Template template, string parameterName)
    {
        if (template == null)
            throw new ValidationException(parameterName + " is required.");

        if (template.AppId < 1)
            throw new ValidationException(parameterName + ".AppId must be greater than 0.");

        if (string.IsNullOrWhiteSpace(template.Name))
            throw new ValidationException(parameterName + ".Name is required.");
    }

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
            throw new ValidationException(message);
    }
}
