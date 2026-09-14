// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Services.Orchestrations;

internal sealed partial class PageRenderCacheOrchestrationService
{


    private static void ValidateAllPageRenderCachesOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidatePageRenderCacheOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidatePageRenderCacheOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidatePageRenderCacheOnUpdate(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidatePageRenderCacheOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAppPageRenderCachesOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAppPageRenderCachesFromEventOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidatePagePageRenderCachesOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidatePagePageRenderCachesFromEventOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidatePageRenderCachesOnReplace(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}