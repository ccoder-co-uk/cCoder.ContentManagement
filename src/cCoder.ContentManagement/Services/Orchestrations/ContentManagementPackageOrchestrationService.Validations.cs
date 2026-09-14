// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Services.Orchestrations;

internal sealed partial class ContentManagementPackageOrchestrationService
{
    private static void ValidateImportPackageAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateExportPackage(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}