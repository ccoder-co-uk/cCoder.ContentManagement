// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class SubmissionEventProcessingService
{
    private static void ValidateRaiseSubmissionAddEventAsync(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateRaiseSubmissionUpdateEventAsync(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateRaiseSubmissionDeleteEventAsync(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}