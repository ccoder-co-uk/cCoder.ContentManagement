// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Services.Aggregations;

internal sealed partial class RenderAggregationService
{
    private static void ValidateRenderPageRenderResponseAsync(
        object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRenderTemplateRenderResult(
        object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRenderComponentRenderResult(
        object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}