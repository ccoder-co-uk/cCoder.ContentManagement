// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Orchestrations.PageContexts;

internal sealed partial class PageContextOrchestrationService
{
    private static void ValidateResolvePageRenderContextAsync(
        object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}