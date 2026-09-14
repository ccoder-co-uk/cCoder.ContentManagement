// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Exposures;
using cCoder.Data.Models.CMS;
using System.ComponentModel.DataAnnotations;

namespace cCoder.ContentManagement.Services.Aggregations;

internal sealed partial class PageRenderCacheAggregationService
{
    private static void ValidatePackageImportOnComplete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidatePageRenderCacheOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidatePageRenderCacheOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidatePageRenderCacheOnUpdate(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidatePageRenderCacheOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAppOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidatePageOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateCommonObjectPageRenderCachesOnRebuild(
        object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAppPageRenderCachesOnRebuild(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidatePagePageRenderCachesOnRebuild(object[] inputs) =>
        Validate(inputs: inputs);

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

    private static void ValidatePageRenderCacheByAppIdOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidatePageRenderCacheByPageIdOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllPageRenderCachesOnRebuild(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateCommonObjectPageRenderOperationOnRebuild(
        object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRenderPageRenderOperation(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRender(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRenderError(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRenderRenderResult(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidatePageRenderCachesByAppIdOnRebuild(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidatePageRenderCachesByPageIdOnRebuild(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}