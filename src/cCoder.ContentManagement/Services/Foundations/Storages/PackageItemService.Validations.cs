// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class PackageItemService
{
    private static void ValidatePackageItemOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllPackageItemOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidatePackageItemOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidatePackageItemOnUpdate(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateDeleteAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}