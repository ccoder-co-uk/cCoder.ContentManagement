// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class ComponentRenderProcessingService
{
    private static void ValidateRenderUser(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateRenderComponentComponentRenderParams(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

}