// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Exposures;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Aggregations;

internal sealed partial class PageRenderAggregationService
{
    private static void ValidateRequest(PageRenderRequest request, string parameterName) =>
        ThrowIf(condition: request == null, message: parameterName + " is required.");

    private static void ValidateAppId(int appId, string parameterName) =>
        ThrowIf(condition: appId < 1, message: parameterName + " must be greater than 0.");

    private static void ValidateApp(App app, string parameterName) =>
        ThrowIf(condition: app == null, message: parameterName + " is required.");

    private static void ValidateTheme(string theme, string parameterName) =>
        ThrowIf(condition: string.IsNullOrWhiteSpace(value: theme), message: parameterName + " is required.");

    private static void ValidateHost(string host) =>
        ThrowIf(condition: string.IsNullOrWhiteSpace(value: host), message: "host is required.");

    private static void ValidateException(Exception exception, string parameterName) =>
        ThrowIf(condition: exception == null, message: parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }
}