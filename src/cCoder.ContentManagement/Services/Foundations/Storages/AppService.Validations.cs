// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class AppService
{
    private static void ValidateId(int appId, string parameterName) =>
        ThrowIf(condition: appId < 1, message: parameterName + " must be greater than 0.");

    private static void ValidateApp(App app, string parameterName)
    {
        if (app == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        if (string.IsNullOrWhiteSpace(value: app.Name))
        {
            throw new ValidationException(message: parameterName + ".Name is required.");
        }

        if (string.IsNullOrWhiteSpace(value: app.Domain))
        {
            throw new ValidationException(message: parameterName + ".Domain is required.");
        }
    }

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