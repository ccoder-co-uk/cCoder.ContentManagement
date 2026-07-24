// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class PackageEventProcessingService
{
    private static void ValidateRaisePackageImportEvent(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateRaisePackageAddEventAsync(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateRaisePackageUpdateEventAsync(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateRaisePackageDeleteEventAsync(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}