// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Services.Processings;

internal partial class PackageEventProcessingService
{
    private static void ValidateRaisePackageAddEventAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRaisePackageUpdateEventAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRaisePackageDeleteEventAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}