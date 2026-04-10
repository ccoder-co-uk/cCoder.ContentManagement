using System.ComponentModel.DataAnnotations;
using Package = cCoder.Data.Models.Packaging.Package;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class PackageService
{
    private static Guid ValidateId(Guid id, string parameterName)
    {
        if (id == Guid.Empty)
        {
            throw new ValidationException(parameterName + " is required.");
        }
        return id;
    }

    private static Package ValidatePackage(Package package, string parameterName)
    {
        if (package == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
        return package;
    }
}
