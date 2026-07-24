// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class PackageService
{
    private static Guid ValidateId(Guid packageId, string parameterName)
    {
        if (packageId == Guid.Empty)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return packageId;
    }

    private static Package ValidatePackage(Package package, string parameterName)
    {
        if (package == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return package;
    }
}