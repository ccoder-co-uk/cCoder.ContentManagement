// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Services.Orchestrations;

internal partial class RenderEventOrchestrationService
{
    private static void ValidateRaiseHttpPageRenderOperationRenderRequestAsync(
        object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}