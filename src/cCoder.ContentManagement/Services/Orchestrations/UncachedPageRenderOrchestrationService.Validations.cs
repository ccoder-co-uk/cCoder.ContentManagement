// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal sealed partial class UncachedPageRenderOrchestrationService
{
    private static void ValidateHttpPageRenderOperationOnRenderAsync(
        object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}