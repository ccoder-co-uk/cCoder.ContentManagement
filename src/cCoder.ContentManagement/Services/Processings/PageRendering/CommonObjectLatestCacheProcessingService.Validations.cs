// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Rendering.Services.Processings;

internal sealed partial class CommonObjectLatestCacheProcessingService
{
    private static void ValidateCommonObjectsOnRefresh(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateLatestCommonObjectsOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}