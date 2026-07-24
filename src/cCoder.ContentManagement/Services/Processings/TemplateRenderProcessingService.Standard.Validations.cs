// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class TemplateRenderProcessingService
{
    private static void ValidateRenderUser(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateRenderTemplateRenderParams(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}
