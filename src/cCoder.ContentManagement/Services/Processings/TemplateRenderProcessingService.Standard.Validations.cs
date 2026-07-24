// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class TemplateRenderProcessingService
{
    private static void ValidateRenderUserConfig(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateRenderTemplateRenderParamsConfig(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}