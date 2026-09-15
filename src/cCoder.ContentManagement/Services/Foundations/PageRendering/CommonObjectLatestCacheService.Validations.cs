// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Rendering.Services.Foundations;

internal sealed partial class CommonObjectLatestCacheService
{
    private static void ValidateCommonObjectsOnRefresh(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateLatestCommonObjectsOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}