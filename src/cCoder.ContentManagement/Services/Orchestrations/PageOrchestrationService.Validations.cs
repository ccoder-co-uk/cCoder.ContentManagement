// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Services.Orchestrations;

internal partial class PageOrchestrationService
{
    private static void ValidatePageForRenderOnGet(
        object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidatePageOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllPageOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidatePageOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidatePageOnUpdate(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateDeleteAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateByAppIdOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateOrUpdatePageResultOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateImportPagesAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllPageOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRecomputeAllForAppAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRootPageOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateChildrenPageOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateMenuFor(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}