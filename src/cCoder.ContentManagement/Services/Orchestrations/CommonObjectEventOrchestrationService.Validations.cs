// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Services.Orchestrations;

internal sealed partial class CommonObjectEventOrchestrationService
{
    private static void ValidateRaiseImported(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRaiseUpdated(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRaiseDeleted(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}