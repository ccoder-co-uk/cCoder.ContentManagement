// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Services.Orchestrations;

internal sealed partial class AppBootstrapOrchestrationService
{
    private static void ValidateNewAppOnPrepare(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAppChildrenOnStamp(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}