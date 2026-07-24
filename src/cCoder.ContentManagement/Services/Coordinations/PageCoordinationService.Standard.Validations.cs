// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Coordinations;

internal partial class PageCoordinationService
{
    private static void ValidateHandlePageAddAsync(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateHandlePageUpdateAsync(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateHandlePageDeleteAsync(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}