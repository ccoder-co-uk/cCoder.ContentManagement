// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Coordinations;

internal sealed partial class PageRenderCoordinationService
{
    private static void ValidateRender(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateRenderError(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateRenderRenderResult(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}