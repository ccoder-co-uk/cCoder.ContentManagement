// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal sealed partial class UncachedPageRenderEventService
{
    private static void ValidateRaiseUncachedPageRenderEventAsync(
        object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}