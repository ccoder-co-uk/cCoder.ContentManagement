// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Services.Processings;

internal partial class PackageItemEventProcessingService
{
    private static void ValidateRaisePackageItemAddEventAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRaisePackageItemUpdateEventAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRaisePackageItemDeleteEventAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}