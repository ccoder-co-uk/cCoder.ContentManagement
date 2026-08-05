// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Processings;

internal sealed partial class PageRenderCacheProcessingService
{


    private static void ValidatePageRenderCacheOnAdd(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidatePageRenderCacheOnUpdate(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidatePageRenderCacheOnDelete(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidatePageRenderCachesOnReplace(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}