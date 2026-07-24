// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Processings;

internal sealed partial class PageRenderProcessingService
{
    private static void ValidateRenderPageUserConfigRenderResult(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}