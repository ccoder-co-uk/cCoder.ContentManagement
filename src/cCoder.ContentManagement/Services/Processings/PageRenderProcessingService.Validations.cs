// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Services.Processings;

internal sealed partial class PageRenderProcessingService
{
    private static void ValidateSerializeRuntimeValue(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRenderPageRenderOperation(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRenderPageUserRenderResult(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}