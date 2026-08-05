// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;
using System.ComponentModel.DataAnnotations;

namespace cCoder.ContentManagement.Services.Foundations.ServiceProviders;

internal sealed partial class ServiceProviderExecutionService
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