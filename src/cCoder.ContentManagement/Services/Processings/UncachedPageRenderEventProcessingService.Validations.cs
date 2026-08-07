// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Processings;

internal sealed partial class UncachedPageRenderEventProcessingService
{
    private static void ValidateRaiseUncachedPageRenderEventAsync(
        object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}