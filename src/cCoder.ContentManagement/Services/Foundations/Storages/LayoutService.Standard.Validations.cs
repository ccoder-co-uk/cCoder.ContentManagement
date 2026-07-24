// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class LayoutService
{
    private static void ValidateLayoutOnGet(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateAllLayoutOnGet(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateLayoutOnAdd(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateLayoutOnUpdate(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateDeleteAsync(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}