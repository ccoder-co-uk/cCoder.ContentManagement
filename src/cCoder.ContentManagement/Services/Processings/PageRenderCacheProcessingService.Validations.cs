// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Services.Processings;

internal sealed partial class PageRenderCacheProcessingService
{


    private static void ValidatePageRenderCacheOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidatePageRenderCacheOnGet(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidatePageRenderCacheOnUpdate(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidatePageRenderCacheOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidatePageRenderCachesOnReplace(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}