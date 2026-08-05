// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Coordinations;

internal sealed partial class PageRenderCoordinationService
{
    private static void ValidateRenderPageRenderResponseAsync(
        object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}