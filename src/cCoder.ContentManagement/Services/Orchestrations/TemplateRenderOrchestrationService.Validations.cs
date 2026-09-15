// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Services.Orchestrations;

internal sealed partial class TemplateRenderOrchestrationService
{
    private static void ValidateRender(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRenderUser(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}