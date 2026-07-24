// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class ComponentService
{
    private static void ValidateId(int componentId, string parameterName) =>
        ThrowIf(condition: componentId < 1, message: parameterName + " must be greater than 0.");

    private static void ValidateComponent(Component component, string parameterName)
    {
        if (component == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        if (component.AppId < 1)
        {
            throw new ValidationException(message: parameterName + ".AppId must be greater than 0.");
        }

        if (string.IsNullOrWhiteSpace(value: component.Name))
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