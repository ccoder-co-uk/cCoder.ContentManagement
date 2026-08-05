// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Aggregations;

internal sealed partial class RenderAggregationService
{
    private static void ValidateRenderPageRenderResponseAsync(
        object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateRenderTemplateRenderResult(
        object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateRenderComponentRenderResult(
        object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}