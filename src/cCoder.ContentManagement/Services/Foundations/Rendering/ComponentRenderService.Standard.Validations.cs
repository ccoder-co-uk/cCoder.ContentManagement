// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Foundations.Rendering;

internal sealed partial class ComponentRenderService
{
    private static void ValidateExecute(object[] inputs) =>
        ValidationRulesEngine.Validate(
            inputs: inputs);

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(value: name))
        {
            throw new ValidationException(
                message: "Service name is required.");
        }
    }

    private static void ValidateOperation(Delegate operation)
    {
        if (operation is null)
        {
            throw new ValidationException(
                message: "Service operation is required.");
        }
    }
}