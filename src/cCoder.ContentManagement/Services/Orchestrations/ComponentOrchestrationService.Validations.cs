// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Services.Orchestrations;

internal partial class ComponentOrchestrationService
{
    private static void ValidateComponentOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllComponentOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateComponentOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateComponentOnUpdate(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateDeleteAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateByAppIdOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateOrUpdateComponentResultOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateImportComponentsAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllComponentOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}