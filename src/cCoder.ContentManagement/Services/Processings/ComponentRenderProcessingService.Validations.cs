// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Services.Processings;

internal partial class ComponentRenderProcessingService
{
    private static void ValidateRenderComponentOperation(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRenderUser(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRenderComponentComponentRenderParams(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}