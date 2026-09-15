// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Services.Processings;

internal partial class TemplateRenderProcessingService
{
    private static void ValidateRenderTemplateOperation(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRenderUser(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRenderTemplateRenderParams(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}