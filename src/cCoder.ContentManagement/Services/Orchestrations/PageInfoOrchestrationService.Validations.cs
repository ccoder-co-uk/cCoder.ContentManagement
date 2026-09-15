// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Services.Orchestrations;

internal partial class PageInfoOrchestrationService
{
    private static void ValidatePageInfoOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllPageInfoOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidatePageInfoOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidatePageInfoOnUpdate(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateDeleteAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateOrUpdatePageInfoResultOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllPageInfoOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}