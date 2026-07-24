// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class ContentService
{
    private static void ValidateId(int contentId, string parameterName) =>
        ThrowIf(condition: contentId < 1, message: parameterName + " must be greater than 0.");

    private static void ValidateContent(Content content, string parameterName)
    {
        if (content == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        if (content.PageId < 1)
        {
            throw new ValidationException(message: parameterName + ".PageId must be greater than 0.");
        }

        if (content.CultureId == null)
        {
            throw new ValidationException(message: parameterName + ".CultureId is required.");
        }

        if (string.IsNullOrWhiteSpace(value: content.Name))
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