// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Models.PageRendering;

namespace cCoder.ContentManagement.Services.Processings.PageRendering;

internal sealed partial class ContentTagHandlingProcessingService
{
    private static void ValidateTagHandlingOperation(
        TagHandlingOperation operation,
        string parameterName)
    {
        if (operation?.Session?.Request is null)
        {
            throw new ValidationException(
                message: $"{parameterName}.Session.Request is required.");
        }

        operation.Content ??= string.Empty;
    }
}