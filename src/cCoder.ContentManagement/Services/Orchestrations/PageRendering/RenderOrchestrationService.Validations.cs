// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Rendering.Services.Orchestrations;

internal sealed partial class RenderOrchestrationService
{
    private static void ValidateRenderRenderSession(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}