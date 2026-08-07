// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Processings;

internal sealed partial class CachedPageRenderProcessingService
{
    private static void ValidatePageRenderCacheOperationOnRender(
        object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}