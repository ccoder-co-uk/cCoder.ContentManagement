// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace cCoder.ContentManagement.Services.Processings.PageRendering;

internal sealed partial class ContentTagHandlingProcessingService
{
    private static void ValidateTagHandlingOperationOnHandle(object[] inputs)
    {
        if (inputs is null || inputs.Length == 0)
        {
            throw new ValidationException(message: "Service inputs are required.");
        }
    }
}