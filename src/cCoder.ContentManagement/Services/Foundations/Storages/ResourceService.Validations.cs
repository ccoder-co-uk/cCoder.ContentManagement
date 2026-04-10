using System.ComponentModel.DataAnnotations;
using Resource = cCoder.Data.Models.CMS.Resource;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class ResourceService
{
    private static void ValidateId(int id, string parameterName)
    {
        if (id < 1)
        {
            throw new ValidationException(parameterName + " must be greater than 0.");
        }
    }

    private static void ValidateResource(Resource resource, string parameterName)
    {
        if (resource == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
        if (resource.AppId < 1)
        {
            throw new ValidationException(parameterName + ".AppId must be greater than 0.");
        }
        if (string.IsNullOrWhiteSpace(resource.Name))
        {
            throw new ValidationException(parameterName + ".Name is required.");
        }
        if (string.IsNullOrWhiteSpace(resource.Key))
        {
            throw new ValidationException(parameterName + ".Key is required.");
        }
    }
}
