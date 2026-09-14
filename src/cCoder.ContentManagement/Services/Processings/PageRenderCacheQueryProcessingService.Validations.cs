// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Services.Processings;

internal sealed partial class PageRenderCacheQueryProcessingService
{
    private static void ValidateAllPageRenderCachesOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidatePageRenderCacheOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}