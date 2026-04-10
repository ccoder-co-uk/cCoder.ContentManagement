using System.ComponentModel.DataAnnotations;
using Component = cCoder.Data.Models.CMS.Component;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class ComponentService
{
    private static void ValidateId(int id, string parameterName)
    {
        if (id < 1)
        {
            throw new ValidationException(parameterName + " must be greater than 0.");
        }
    }

    private static void ValidateComponent(Component component, string parameterName)
    {
        if (component == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
        if (component.AppId < 1)
        {
            throw new ValidationException(parameterName + ".AppId must be greater than 0.");
        }
        if (string.IsNullOrWhiteSpace(component.Name))
        {
            throw new ValidationException(parameterName + ".Name is required.");
        }
    }
}
