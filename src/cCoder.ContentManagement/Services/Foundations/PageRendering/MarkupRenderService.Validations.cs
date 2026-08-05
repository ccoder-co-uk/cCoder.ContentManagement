// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Rendering.Services.Foundations;

internal sealed partial class MarkupRenderService
{
    private static void ValidateRenderRenderSessionReplacementDependencies(
        object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}