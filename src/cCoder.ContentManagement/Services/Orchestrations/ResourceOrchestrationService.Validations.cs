// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Services.Orchestrations;

internal partial class ResourceOrchestrationService
{
    private static void ValidateResourceOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllResourceOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateResourceOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateResourceOnUpdate(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateDeleteAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateByAppIdOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateOrUpdateResourceResultOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateImportResourcesAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllResourceOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}