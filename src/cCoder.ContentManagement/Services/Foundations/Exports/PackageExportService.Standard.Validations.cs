// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Foundations.Exports;

internal partial class PackageExportService
{
    private static void ValidateExportRolesPackage(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateExportLayoutsPackage(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateExportTemplatesPackage(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateExportComponentsPackage(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateExportScriptsPackage(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateExportResourcesPackage(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateExportPagesPackage(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateExportPageRolesPackage(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}