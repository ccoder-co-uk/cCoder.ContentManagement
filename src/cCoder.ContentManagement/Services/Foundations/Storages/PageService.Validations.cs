// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class PageService
{
    private static void ValidateId(int pageId, string parameterName) =>
        ThrowIf(condition: pageId < 1, message: parameterName + " must be greater than 0.");

    private static void ValidatePage(Page page, string parameterName)
    {
        if (page == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        if (page.AppId < 1)
        {
            throw new ValidationException(message: parameterName + ".AppId must be greater than 0.");
        }

        if (string.IsNullOrWhiteSpace(value: page.Name))
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