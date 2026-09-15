// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Services.Orchestrations;

internal partial class SubmissionOrchestrationService
{
    private static void ValidateSubmissionOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllSubmissionOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateSubmissionOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateSubmissionOnUpdate(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateDeleteAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateOrUpdateSubmissionResultOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllSubmissionOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}