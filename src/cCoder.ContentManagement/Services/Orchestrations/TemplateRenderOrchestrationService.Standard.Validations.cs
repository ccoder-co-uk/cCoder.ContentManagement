// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal sealed partial class TemplateRenderOrchestrationService
{
    private static void ValidateRenderUser(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}