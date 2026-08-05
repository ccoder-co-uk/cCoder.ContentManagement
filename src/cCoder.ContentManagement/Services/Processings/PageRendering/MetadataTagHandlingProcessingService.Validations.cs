// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Models.PageRendering;

namespace cCoder.ContentManagement.Services.Processings.PageRendering;

internal sealed partial class MetadataTagHandlingProcessingService
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

        if (operation.Session?.MetadataResolver is null)
        {
            throw new ValidationException(
                message: $"{parameterName}.Session.MetadataResolver is required.");
        }

        operation.Content ??= string.Empty;
    }
}