// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Services.Processings;

internal partial class PackageProcessingService
{
    private static void ValidateExportPackage(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateExportPackages(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidatePackageOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllPackageOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidatePackageOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidatePackageOnUpdate(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateDeleteAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateOrUpdatePackageResultOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllPackageOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}