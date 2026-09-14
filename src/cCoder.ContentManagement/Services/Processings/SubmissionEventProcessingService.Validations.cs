// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Services.Processings;

internal partial class SubmissionEventProcessingService
{
    private static void ValidateRaiseSubmissionAddEventAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRaiseSubmissionUpdateEventAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRaiseSubmissionDeleteEventAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}