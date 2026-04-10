using System.ComponentModel.DataAnnotations;
using PageRole = cCoder.Data.Models.Security.PageRole;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class PageRoleService
{
    private static void ValidatePageId(int pageId, string parameterName)
    {
        if (pageId < 1)
        {
            throw new ValidationException(parameterName + " must be greater than 0.");
        }
    }

    private static void ValidateRoleId(Guid roleId, string parameterName)
    {
        if (roleId == Guid.Empty)
        {
            throw new ValidationException(parameterName + " is required.");
        }
    }

    private static void ValidatePageRole(PageRole pageRole, string parameterName)
    {
        if (pageRole == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
        if (pageRole.PageId < 1)
        {
            throw new ValidationException(parameterName + ".PageId must be greater than 0.");
        }
        if (pageRole.RoleId == Guid.Empty)
        {
            throw new ValidationException(parameterName + ".RoleId is required.");
        }
    }
}
