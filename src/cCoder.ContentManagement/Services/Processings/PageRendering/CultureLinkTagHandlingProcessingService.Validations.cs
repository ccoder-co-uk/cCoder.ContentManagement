// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.PageRendering;
using System.ComponentModel.DataAnnotations;

namespace cCoder.ContentManagement.Services.Processings.PageRendering;

internal sealed partial class CultureLinkTagHandlingProcessingService
{
    private static void ValidateTagHandlingOperation(
        TagHandlingOperation operation,
        string parameterName)
    {
        if (operation is null)
        {
            throw new ValidationException(
                message: $"{parameterName} is required.");
        }

        operation.Content ??= string.Empty;
    }

    private static void ValidateTagHandlingOperationOnHandle(object[] inputs)
    {
        if (inputs is null || inputs.Length == 0)
        {
            throw new ValidationException(message: "Service inputs are required.");
        }
    }
}