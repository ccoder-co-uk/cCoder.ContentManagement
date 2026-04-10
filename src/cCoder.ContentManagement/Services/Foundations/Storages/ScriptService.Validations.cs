using System.ComponentModel.DataAnnotations;
using Script = cCoder.Data.Models.CMS.Script;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class ScriptService
{
    private static void ValidateId(int id, string parameterName)
    {
        if (id < 1)
        {
            throw new ValidationException(parameterName + " must be greater than 0.");
        }
    }

    private static void ValidateScript(Script script, string parameterName)
    {
        if (script == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
        if (script.AppId < 1)
        {
            throw new ValidationException(parameterName + ".AppId must be greater than 0.");
        }
        if (string.IsNullOrWhiteSpace(script.Name))
        {
            throw new ValidationException(parameterName + ".Name is required.");
        }
    }
}
