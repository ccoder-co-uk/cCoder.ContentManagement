// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal partial class MigrationSupportOrchestrationService
{
    private static void ValidateDeserializeItems(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateExportPackages(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}