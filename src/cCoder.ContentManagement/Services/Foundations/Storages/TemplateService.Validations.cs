using System.ComponentModel.DataAnnotations;
using Template = cCoder.Data.Models.CMS.Template;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class TemplateService
{
    private static void ValidateId(int id, string parameterName)
    {
        if (id < 1)
        {
            throw new ValidationException(parameterName + " must be greater than 0.");
        }
    }

    private static void ValidateTemplate(Template template, string parameterName)
    {
        if (template == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
        if (template.AppId < 1)
        {
            throw new ValidationException(parameterName + ".AppId must be greater than 0.");
        }
        if (string.IsNullOrWhiteSpace(template.Name))
        {
            throw new ValidationException(parameterName + ".Name is required.");
        }
    }
}
