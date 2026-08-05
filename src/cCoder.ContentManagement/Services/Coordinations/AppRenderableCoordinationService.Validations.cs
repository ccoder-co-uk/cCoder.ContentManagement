// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Coordinations;

internal partial class AppRenderableCoordinationService
{
    private static void ValidateHandleAppAddAsync(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateHandleAppUpdateAsync(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateHandleAppDeleteAsync(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}