// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal sealed partial class PageRenderCacheService
{
    private static void ValidateId(string pageRenderCacheId, string parameterName) =>
        ThrowIf(
            condition: string.IsNullOrWhiteSpace(value: pageRenderCacheId),
            message: parameterName + " is required.");

    private static void ValidateId(int pageRenderCacheId, string parameterName) =>
        ThrowIf(condition: pageRenderCacheId < 1, message: parameterName + " must be greater than 0.");

    private static void ValidatePageRenderCache(PageRenderCache cache, string parameterName)
    {
        ThrowIf(condition: cache == null, message: parameterName + " is required.");
        ThrowIf(condition: string.IsNullOrWhiteSpace(value: cache.Id), message: parameterName + ".Id is required.");
        ThrowIf(condition: cache.AppId < 1, message: parameterName + ".AppId must be greater than 0.");
        ThrowIf(condition: cache.PageId < 1, message: parameterName + ".PageId must be greater than 0.");
        ThrowIf(condition: string.IsNullOrWhiteSpace(value: cache.Theme), message: parameterName + ".Theme is required.");
        ThrowIf(condition: cache.Path == null, message: parameterName + ".Path is required.");
        ThrowIf(condition: cache.Header == null, message: parameterName + ".Header is required.");
        ThrowIf(condition: cache.Body == null, message: parameterName + ".Body is required.");
    }

    private static void ValidateReplacementInputs(int[] pageIds, PageRenderCache[] replacements)
    {
        ThrowIf(condition: replacements == null, message: "replacements is required.");

        if (pageIds != null && pageIds.Any(predicate: pageId => pageId < 1))
        {
            throw new ValidationException(message: "pageIds must contain values greater than 0.");
        }

        foreach (PageRenderCache replacement in replacements)
        {
            ValidatePageRenderCache(cache: replacement, parameterName: "replacement");
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