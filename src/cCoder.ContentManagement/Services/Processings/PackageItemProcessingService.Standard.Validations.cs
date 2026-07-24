// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class PackageItemProcessingService
{
    private static void ValidatePackageItemOnGet(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateAllPackageItemOnGet(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidatePackageItemOnAdd(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidatePackageItemOnUpdate(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateDeleteAsync(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateOrUpdatePackageItemResultOnAdd(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateAllPackageItemOnDelete(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}