// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal partial class PageRenderOrchestrationService
{
    private static void ValidateRenderPageUserRenderResult(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}