// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Dependencies;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal sealed partial class CachedPageRenderOrchestrationService
{
    private static void ValidateHttpPageRenderOperationOnRenderAsync(
            object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateCachedPageRenderOperationOnRender(
        object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateCachedPageRenderOperation(
        CachedPageRenderOperation operation,
        string parameterName)
    {
        if (operation is null)
        {
            throw new ValidationException(
                message: parameterName + " is required.");
        }

        if (operation.Page is null)
        {
            throw new ValidationException(message: "Page is required.");
        }

        if (string.IsNullOrWhiteSpace(value: operation.Theme))
        {
            throw new ValidationException(message: "Theme is required.");
        }

        operation.Culture ??= string.Empty;
    }
}