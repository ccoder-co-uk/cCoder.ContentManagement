// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class PageProcessingService
{
    private static void ValidateId(int pageId, string parameterName) =>
        ThrowIf(condition: pageId < 1, message: parameterName + " must be greater than 0.");

    private static void ValidateAppId(int appId, string parameterName) =>
        ThrowIf(condition: appId < 1, message: parameterName + " must be greater than 0.");

    private static void ValidatePage(Page page, string parameterName) =>
        ThrowIf(condition: page == null, message: parameterName + " is required.");

    private static void ValidatePages(IEnumerable<Page> pages, string parameterName) =>
        ThrowIf(condition: pages == null, message: parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }
}