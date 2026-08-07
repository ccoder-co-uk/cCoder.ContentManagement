// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal sealed partial class CachedPageRenderOrchestrationService
{
    private static void ValidateHttpPageRenderOperationOnRender(
        object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}