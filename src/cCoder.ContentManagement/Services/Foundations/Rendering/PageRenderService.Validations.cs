// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Foundations.Rendering;

internal sealed partial class PageRenderService
{
    private static void ValidatePageRenderFoundationOperation(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}