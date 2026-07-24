// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class CultureService
{
    private static void ValidateId(string id, string parameterName) =>
        ThrowIf(condition: string.IsNullOrWhiteSpace(value: id), message: parameterName + " is required.");

    private static void ValidateCulture(Culture culture, string parameterName)
    {
        if ((object)culture == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        if (string.IsNullOrWhiteSpace(value: culture.Id))
        {
            throw new ValidationException(message: parameterName + ".Id is required.");
        }

        if (string.IsNullOrWhiteSpace(value: culture.Name))
        {
            throw new ValidationException(message: parameterName + ".Name is required.");
        }
    }

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }
}