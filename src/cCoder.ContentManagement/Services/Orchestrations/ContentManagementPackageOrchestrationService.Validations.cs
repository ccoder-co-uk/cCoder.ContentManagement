// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal sealed partial class ContentManagementPackageOrchestrationService
{
    private static void ValidateImportPackageAsync(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateExportPackage(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}