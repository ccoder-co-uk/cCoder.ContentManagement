// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;
using cCoder.Data.Models.CMS;
using System.ComponentModel.DataAnnotations;

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

    private static void ValidatePageForRenderOnGet(
        object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidatePageOnGet(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateAllPageOnGet(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateMenuFor(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateRootPageOnGet(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateChildrenPageOnGet(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateDeleteAsync(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidatePageOnUpdate(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidatePageOnAdd(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateOrUpdatePageResultOnAdd(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateAllPageOnDelete(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateRecomputeAllForAppAsync(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}