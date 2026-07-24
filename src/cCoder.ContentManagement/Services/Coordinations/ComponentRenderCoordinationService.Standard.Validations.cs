// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Coordinations;

internal sealed partial class ComponentRenderCoordinationService
{
    private static void ValidateRender(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}